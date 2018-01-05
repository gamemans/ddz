﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public int basePointPerMatch;//底分
    private int multiples;//全场倍数

	public GameObject m_Canvas;
    // Use this for initialization

	public Dictionary<string, bool> pokeflag;
    void Start()
    {
		m_Canvas = GameObject.Find("Canvas");
        multiples = 1;
        basePointPerMatch = 100;
        InitMenu();
    }

    /// <summary>
    /// 全场倍数
    /// </summary>
    public int Multiples
    {
        set { multiples *= value; }
        get { return multiples; }
    }

    /// <summary>
    /// 初始化菜单面板
    /// </summary>
    public void InitMenu()
    {

		GameObject panel = Instantiate (Resources.Load<GameObject> ("mystartpanel"));
	
		panel.transform.SetParent(m_Canvas.transform, false);

        panel.AddComponent<Menu>();

    }

	public void Startgame()
	{
		Debug.Log ("===1===1==");
	}

    /// <summary>
    /// 初始化交互面板
    /// </summary>
    public void InitInteraction()
    {

//		GameObject iteraction = Instantiate (Resources.Load<GameObject> ("InteractionPanel"));
//		iteraction.transform.SetParent(GameObject.Find("Canvas").transform, false);
//
//        iteraction.name = iteraction.name.Replace("(Clone)", "");
//        iteraction.AddComponent<Interaction>();
    }

    /// <summary>
    /// 初始化场景個数据
    /// </summary>
    public void InitScene()
    {
        string fileName = "";
        if (Application.platform == RuntimePlatform.Android)
        {
            fileName = Application.persistentDataPath;
        }
        else
        {
            fileName = Application.dataPath;
        }
        FileInfo info = new FileInfo(fileName + @"\data.json");

      //  GameObject bg = NGUITools.AddChild(UICamera.mainCamera.gameObject,
   // (GameObject)Resources.Load("BackgroundPanel"));

		GameObject bg = Instantiate (Resources.Load<GameObject> ("BackgroundPanel"));
		bg.transform.SetParent(GameObject.Find("Canvas").transform, false);
        bg.name = bg.name.Replace("(Clone)", "");


		GameObject scene = Instantiate (Resources.Load<GameObject> ("myScenePanel"));
		scene.transform.SetParent(m_Canvas.transform, false);
		scene.name = scene.name.Replace("(Clone)", "");

		scene.AddComponent<Interaction>();

        GameObject player = scene.transform.Find("Player").gameObject;
        HandCards playerCards = player.AddComponent<HandCards>();
        playerCards.cType = CharacterType.Player;
        player.AddComponent<PlayCard>();

        GameObject computerOne = scene.transform.Find("ComputerOne").gameObject;
        HandCards computerOneCards = computerOne.AddComponent<HandCards>();
        computerOneCards.cType = CharacterType.ComputerOne;
        computerOne.AddComponent<SimpleSmartCard>();
     //   computerOne.transform.Find("ComputerNotice").gameObject.SetActive(false);

        GameObject computerTwo = scene.transform.Find("ComputerTwo").gameObject;
        HandCards computerTwoCards = computerTwo.AddComponent<HandCards>();
        computerTwoCards.cType = CharacterType.ComputerTwo;
        computerTwo.AddComponent<SimpleSmartCard>();
   //     computerTwo.transform.Find("ComputerNotice").gameObject.SetActive(false);

        GameObject desk = scene.transform.Find("Desk").gameObject;
        desk.transform.Find("NoticeLabel").gameObject.SetActive(false);

        if (!info.Exists)
        {
            playerCards.Integration = 1000;
            computerOneCards.Integration = 1000;
            computerTwoCards.Integration = 1000;
        }
        else
        {
            GameData data = GetDataWithoutBOM(fileName);

            playerCards.Integration = data.playerIntegration;
            computerOneCards.Integration = data.computerOneIntegration;
            computerTwoCards.Integration = data.computerTwoIntegration;
        }

        GameController.UpdateIntegration(CharacterType.Player);
        GameController.UpdateIntegration(CharacterType.ComputerOne);
        GameController.UpdateIntegration(CharacterType.ComputerTwo);

    }

    /// <summary>
    /// 洗牌
    /// </summary>
    public void DealCards()
    {
        //洗牌
        Deck.Instance.Shuffle();

        //发牌
        CharacterType currentCharacter = CharacterType.Player;
        for (int i = 0; i < 51; i++)
        {
            if (currentCharacter == CharacterType.Desk)
            {
                currentCharacter = CharacterType.Player;
            }
            DealTo(currentCharacter);
            currentCharacter++;
        }

        for (int i = 0; i < 3; i++)
        {
            DealTo(CharacterType.Desk);
        }

        //
        for (int i = 1; i < 5; i++)
        {
            MakeHandCardsSprite((CharacterType)i, false);
        }
    }

    /// <summary>
    /// 游戏结束
    /// </summary>
    public void GameOver()
    {
        GameObject currentCharactor = GameObject.Find(OrderController.Instance.Type.ToString());
        Identity identity = currentCharactor.GetComponent<HandCards>().AccessIdentity;

        //统计所有人的分数
        for (int i = 1; i < 4; i++)
        {
            StatisticalIntegral((CharacterType)i, identity);
        }

        //结束界面
//        GameObject gameover = NGUITools.AddChild(UICamera.mainCamera.gameObject,
//    (GameObject)Resources.Load("GameOverPanel"));

		GameObject gameover = Instantiate (Resources.Load<GameObject> ("GameOverPanel"));
		gameover.transform.SetParent(GameObject.Find("Canvas").transform);

        gameover.AddComponent<Restart>();

        int playerIntegration = GameObject.Find(CharacterType.Player.ToString()).GetComponent<HandCards>().Integration;
        int computerOneIntegration = GameObject.Find(CharacterType.ComputerOne.ToString()).GetComponent<HandCards>().Integration;
        int computerTwoIntegration = GameObject.Find(CharacterType.ComputerTwo.ToString()).GetComponent<HandCards>().Integration;
        if (playerIntegration > 0)
        {
            gameover.GetComponent<Restart>().SetTimeToNext(3.0f);

            if (computerOneIntegration <= 0)
            {
                StartCoroutine(ChangeComputer(CharacterType.ComputerOne));
            }

            if (computerTwoIntegration <= 0)
            {
                StartCoroutine(ChangeComputer(CharacterType.ComputerTwo));
            }

            DisplayOverInfo(true, gameover, identity);
            //数据存储
            GameData data = new GameData
            {

                playerIntegration = playerIntegration > 0 ? playerIntegration : 1000,
                computerOneIntegration = computerOneIntegration > 0 ? computerOneIntegration : 1000,
                computerTwoIntegration = computerTwoIntegration > 0 ? computerTwoIntegration : 1000
            };
            SaveDataWithUTF8(data);
        }
        else
        {
            DisplayOverInfo(false, gameover, identity);
            //数据存储
            GameData data = new GameData
            {
                playerIntegration = 1000,
                computerOneIntegration = 1000,
                computerTwoIntegration = 1000
            };
            SaveDataWithUTF8(data);
        }
    }

    /// <summary>
    /// 统计积分
    /// </summary>
    /// <param name="type"></param>
    /// <param name="winner"></param>
    void StatisticalIntegral(CharacterType type, Identity winner)
    {
        //积分计算
        HandCards cards = GameObject.Find(type.ToString()).GetComponent<HandCards>();

        int integration = cards.Multiples * Multiples * basePointPerMatch * (int)(cards.AccessIdentity + 1);

        if (cards.AccessIdentity != winner)
            integration = -integration;

        cards.Integration = cards.Integration + integration;

        //更新显示
        UpdateIntegration(type);
    }

    /// <summary>
    /// 更换电脑玩家
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    IEnumerator ChangeComputer(CharacterType type)
    {
        DisplayDeskNotice(true);
        yield return new WaitForSeconds(3.0f);
        GameObject oldComputer = GameObject.Find(type.ToString());
        Destroy(oldComputer);
        MatchNewComputer(type);
    }

    /// <summary>
    /// 匹配新的电脑玩家
    /// </summary>
    /// <param name="type"></param>
    void MatchNewComputer(CharacterType type)
    {
        BackToDeck();
        DestroyAllSprites();
        DeskCardsCache.Instance.Clear();
        OrderController.Instance.ResetSmartCard();

        DisplayDeskNotice(false);
		GameObject newComputer = Instantiate (Resources.Load<GameObject> ("myScenePanel"));
		newComputer.transform.SetParent(GameObject.Find("Canvas").transform);

		newComputer.name = newComputer.name.Replace("(Clone)", "");
        newComputer.AddComponent<HandCards>().cType = type;
        newComputer.AddComponent<HandCards>().Integration = 1000;
        newComputer.transform.Find("ComputerNotice").gameObject.SetActive(false);
        newComputer.AddComponent<SimpleSmartCard>();
        if (Random.Range(1, 3) == 1)
        {
			//ww
          //  newComputer.transform.Find("HeadPortrait").gameObject.GetComponent<UISprite>().spriteName = "role1";
        }
        else
        {
			//ww
          //  newComputer.transform.Find("HeadPortrait").gameObject.GetComponent<UISprite>().spriteName = "role";
        }
    }

    /// <summary>
    /// 存储数据
    /// </summary>
    /// <param name="data"></param>
    void SaveDataWithUTF8(GameData data)
    {
        string fileName = "";
        if (Application.platform == RuntimePlatform.Android)
        {
            fileName = Application.persistentDataPath;
        }
        else
        {
            fileName = Application.dataPath;
        }

        Stream stream = new FileStream(fileName + @"\data.json", FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
        StreamWriter writter = new StreamWriter(stream, Encoding.UTF8);
        XmlSerializer xmlSerializer = new XmlSerializer(data.GetType());
        xmlSerializer.Serialize(writter, data);
        writter.Close();
        stream.Close();
    }

    /// <summary>
    /// 跳过utf-8 xml BOM的方法
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    GameData GetDataWithoutBOM(string fileName)
    {
        GameData data = new GameData();
        Stream stream = new FileStream(fileName + @"\data.json", FileMode.Open, FileAccess.Read, FileShare.None);
        StreamReader streamReader = new StreamReader(stream, true);//true跳过bom

        XmlSerializer xmlSerializer = new XmlSerializer(data.GetType());
        data = xmlSerializer.Deserialize(streamReader) as GameData;
        streamReader.Close();
        stream.Close();

        return data;
    }

    /// <summary>
    /// 场上的所有牌回到牌库中
    /// </summary>
    public void BackToDeck()
    {
        HandCards[] handCards = new HandCards[3];
        handCards[0] = GameObject.Find("Player").GetComponent<HandCards>();
        handCards[1] = GameObject.Find("ComputerOne").GetComponent<HandCards>();
        handCards[2] = GameObject.Find("ComputerTwo").GetComponent<HandCards>();

        for (int i = 0; i < handCards.Length; i++)
        {
            while (handCards[i].CardsCount != 0)
            {
                Card last = handCards[i][handCards[i].CardsCount - 1];
                Deck.Instance.AddCard(last);
                handCards[i].PopCard(last);
            }
        }
    }

    /// <summary>
    /// 销毁精灵
    /// </summary>
    public void DestroyAllSprites()
    {
        CardSprite[][] cardSprites = new CardSprite[3][];
        cardSprites[0] = GameObject.Find("Player").GetComponentsInChildren<CardSprite>();
        cardSprites[1] = GameObject.Find("ComputerOne").GetComponentsInChildren<CardSprite>();
        cardSprites[2] = GameObject.Find("ComputerTwo").GetComponentsInChildren<CardSprite>();

        for (int i = 0; i < cardSprites.GetLength(0); i++)
        {
            for (int j = 0; j < cardSprites[i].Length; j++)
            {
                cardSprites[i][j].Destroy();
            }
        }
    }

    /// <summary>
    /// 发底牌
    /// </summary>
    /// <param name="type"></param>
    public void CardsOnTable(CharacterType type)
    {
        GameObject parentObj = GameObject.Find(type.ToString());
        HandCards cards = parentObj.GetComponent<HandCards>();
        //积分翻倍
        cards.Multiples = 2;
        //销毁底牌精灵
        CardSprite[] sprites = GameObject.Find("Desk").GetComponentsInChildren<CardSprite>();
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].Destroy();
        }

        while (DeskCardsCache.Instance.CardsCount != 0)
        {
            Card card = DeskCardsCache.Instance.Deal();
            cards.AddCard(card);
        }
        MakeHandCardsSprite(type, true);
        //更新身份
        UpdateIndentity(type, Identity.Landlord);
    }

    /// <summary>
    /// 发牌
    /// </summary>
    /// <param name="person"></param>
    void DealTo(CharacterType person)
    {
        if (person == CharacterType.Desk)
        {
            Card movedCard = Deck.Instance.Deal();
            DeskCardsCache.Instance.AddCard(movedCard);
        }
        else
        {
            GameObject playerObj = GameObject.Find(person.ToString());
            HandCards cards = playerObj.GetComponent<HandCards>();

            Card movedCard = Deck.Instance.Deal();
            cards.AddCard(movedCard);
        }
    }

    /// <summary>
    /// 使卡牌精灵化
    /// </summary>
    /// <param name="type"></param>
    /// <param name="card"></param>
    /// <param name="selected"></param>
    void MakeSprite(CharacterType type, Card card, bool selected, int i)
    {

		Debug.Log (string.Format("MakeSprite11==={0}-{1}", card.GetCardSuit, card.GetCardWeight));

        if (!card.isSprite)
        {
			GameObject obj = Resources.Load<GameObject>(string.Format("poke_prefab/Poke_{0}_{1}", (int)card.GetCardSuit, (int)card.GetCardWeight)) as GameObject;
           // GameObject poker = NGUITools.AddChild(GameObject.Find(type.ToString()), obj);

			GameObject player = GameObject.Find(type.ToString());

			GameObject poker = Instantiate(obj);
			poker.transform.position = new Vector3(i * 25 - 200, 0, 0);
			poker.transform.SetParent(player.transform, false);


			CardSprite sprite = poker.AddComponent<CardSprite>();
			sprite.Poker = card;
			sprite.Select = selected;

			Button pokebtn = poker.transform.Find("Button").gameObject.GetComponent<Button>();
//			pokebtn.name = string.Format("pokebtn_{0}_{1}",card.GetCardSuit, card.GetCardWeight);
//			pokeflag.Add(pokebtn.name, false);
//			pokebtn.onClick.AddListener(delegate() {
//				this.OnPokeClick(pokebtn);
//			});

//            CardSprite sprite = poker.gameObject.GetComponent<CardSprite>();
//            sprite.Poker = card;
//            sprite.Select = selected;
        }
    }

	void OnPokeClick(Button btn)
	{
		if (pokeflag[btn.name])
		{
			transform.localPosition -= Vector3.up * 10;
			pokeflag[btn.name] = false;
		}
		else
		{
			transform.localPosition += Vector3.up * 10;
			pokeflag[btn.name] = true;
		}
	
	}
    /// <summary>
    /// 精灵化角色手牌
    /// </summary>
    /// <param name="type"></param>
    /// <param name="isSelected"></param>
    void MakeHandCardsSprite(CharacterType type, bool isSelected)
    {
        if (type == CharacterType.Desk)
        {
            DeskCardsCache instance = DeskCardsCache.Instance;
            for (int i = 0; i < instance.CardsCount; i++)
            {
                MakeSprite(type, instance[i], isSelected, i);
            }
        }
        else
        {
            GameObject parentObj = GameObject.Find(type.ToString());
            HandCards cards = parentObj.GetComponent<HandCards>();
            //排序
            cards.Sort();
            //精灵化
            for (int i = 0; i < cards.CardsCount; i++)
            {
                if (!cards[i].isSprite)
                {
                    MakeSprite(type, cards[i], isSelected, i);
                }
            }

            //显示剩余扑克
            UpdateLeftCardsCount(cards.cType, cards.CardsCount);
        }
        //调整精灵位置
        AdjustCardSpritsPosition(type);
    }


    /// <summary>
    /// 调整手牌位置
    /// </summary>
    /// <param name="type"></param>
    public static void AdjustCardSpritsPosition(CharacterType type)
    {
        if (type == CharacterType.Desk)
        {
            DeskCardsCache instance = DeskCardsCache.Instance;
            CardSprite[] cs = GameObject.Find(type.ToString()).GetComponentsInChildren<CardSprite>();
            for (int i = 0; i < cs.Length; i++)
            {
                for (int j = 0; j < cs.Length; j++)
                {
                    if (cs[j].Poker == instance[i])
                    {
                        cs[j].GoToPosition(GameObject.Find(type.ToString()), i);
                    }
                }
            }
        }
        else
        {
            HandCards hc = GameObject.Find(type.ToString()).GetComponent<HandCards>();
            CardSprite[] cs = GameObject.Find(type.ToString()).GetComponentsInChildren<CardSprite>();
            for (int i = 0; i < hc.CardsCount; i++)
            {
                for (int j = 0; j < cs.Length; j++)
                {
                    if (cs[j].Poker == hc[i])
                    {
                        cs[j].GoToPosition(GameObject.Find(type.ToString()), i);
                    }
                }
            }
        }

    }

    /// <summary>
    /// 获取指定数组的权值
    /// </summary>
    /// <param name="cards"></param>
    /// <param name="rule"></param>
    /// <returns></returns>
    public static int GetWeight(Card[] cards, CardsType rule)
    {
        int totalWeight = 0;
        if (rule == CardsType.ThreeAndOne || rule == CardsType.ThreeAndTwo)
        {
            for (int i = 0; i < cards.Length; i++)
            {
                if (i < cards.Length - 2)
                {
                    if (cards[i].GetCardWeight == cards[i + 1].GetCardWeight &&
                        cards[i].GetCardWeight == cards[i + 2].GetCardWeight)
                    {
                        totalWeight += (int)cards[i].GetCardWeight;
                        totalWeight *= 3;
                        break;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < cards.Length; i++)
            {
                totalWeight += (int)cards[i].GetCardWeight;
            }
        }

        return totalWeight;
    }

    /// <summary>
    /// 更新剩余扑克显示
    /// </summary>
    /// <param name="type"></param>
    /// <param name="cardsCount"></param>
    public static void UpdateLeftCardsCount(CharacterType type, int cardsCount)
    {
        GameObject obj = GameObject.Find(type.ToString()).transform.FindChild("LeftPoker").gameObject;

		Text txt = obj.GetComponent<Text> ();
		txt.text = string.Format ("剩余扑克:{0}", cardsCount);
		//ww
      //  obj.GetComponent<UILabel>().text = "剩余扑克:" + cardsCount;
    }

    /// <summary>
    /// 更新身份
    /// </summary>
    /// <param name="type"></param>
    /// <param name="identity"></param>
    public static void UpdateIndentity(CharacterType type, Identity identity)
    {
        GameObject obj = GameObject.Find(type.ToString()).transform.FindChild("Identity").gameObject;
        //改变属性
        GameObject.Find(type.ToString()).GetComponent<HandCards>().AccessIdentity = identity;
        //更改显示
       //ww obj.GetComponent<UISprite>().spriteName = "Identity_" + identity.ToString();
    }

    /// <summary>
    /// 更新积分显示
    /// </summary>
    /// <param name="type"></param>
    /// <param name="integration"></param>
    public static void UpdateIntegration(CharacterType type)
    {
		
		Debug.Log(string.Format("UpdateIntegration==={0}", type.ToString()));
		string str = type.ToString ();
		GameObject obj = GameObject.Find (type.ToString ());
        int integration = obj.GetComponent<HandCards>().Integration;
        GameObject obj1 = GameObject.Find(type.ToString()).transform.FindChild("IntegrationLabel").gameObject;

		Text txt = obj1.GetComponent<Text> ();
		txt.text = string.Format ("积分:{0}", integration);
       //ww obj.GetComponent<UILabel>().text = "积分:" + integration;
    }

    /// <summary>
    /// 电脑玩家离开提示
    /// </summary>
    /// <param name="show"></param>
    public static void DisplayDeskNotice(bool show)
    {
        GameObject.Find("Desk").transform.Find("NoticeLabel").gameObject.SetActive(show);
    }

    /// <summary>
    /// 显示结束信息
    /// </summary>
    /// <param name="enough"></param>
    /// <param name="gameovePanel"></param>
    /// <param name="winner"></param>
    public static void DisplayOverInfo(bool enough, GameObject gameovePanel, Identity winner)
    {
        if (enough)
        {
            gameovePanel.transform.FindChild("Button").gameObject.SetActive(false);
            gameovePanel.GetComponent<Restart>().SetTimeToNext(3.0f);
        }
        if (winner == Identity.Farmer)
        {
           //ww gameovePanel.GetComponentInChildren<UISprite>().spriteName = "role1";
          //ww  gameovePanel.GetComponentInChildren<UILabel>().text = "农民获得胜利";
        }
        else
        {
         //ww   gameovePanel.GetComponentInChildren<UISprite>().spriteName = "role";
          //ww  gameovePanel.GetComponentInChildren<UILabel>().text = "地主获得胜利";
        }
    }

//	[MenuItem ("MyMenu/Build Assetbundle")]
//	static private void BuildAssetBundle()
//	{
//		string dir = Application.dataPath +"/StreamingAssets";
//		
//		if(!Directory.Exists(dir)){
//			Directory.CreateDirectory(dir);
//		}
//		DirectoryInfo rootDirInfo = new DirectoryInfo (Application.dataPath +"/Atlas");
//		foreach (DirectoryInfo dirInfo in rootDirInfo.GetDirectories()) {
//			List<Sprite> assets = new List<Sprite>();
//			string path = dir +"/"+dirInfo.Name+".assetbundle";
//			foreach (FileInfo pngFile in dirInfo.GetFiles("*.png",SearchOption.AllDirectories)) 
//			{
//				string allPath = pngFile.FullName;
//				string assetPath = allPath.Substring(allPath.IndexOf("Assets"));
//				assets.Add(Resources.Load<Sprite>(assetPath));
//			}
//			if(BuildPipeline.BuildAssetBundle(null, assets.ToArray(), path,BuildAssetBundleOptions.UncompressedAssetBundle| BuildAssetBundleOptions.CollectDependencies, GetBuildTarget())){
//			}
//		}	
//	}
//	static private BuildTarget GetBuildTarget()
//	{
//		BuildTarget target = BuildTarget.WebPlayer;
//		#if UNITY_STANDALONE
//		target = BuildTarget.StandaloneWindows;
//		#elif UNITY_IPHONE
//		target = BuildTarget.iPhone;
//		#elif UNITY_ANDROID
//		target = BuildTarget.Android;
//		#endif
//		return target;
//	}
}
