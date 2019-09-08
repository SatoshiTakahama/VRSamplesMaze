using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine.AI;
using System;
using VRStandardAssets.Maze;
using VRM;

public class ChangeCharacterUI : MonoBehaviour
{
    bool inMenu;
    private Text sliderText;
    private Text selectText;
	private int selectCharacter;
	private int currentCharacter;
	private String TitleText;
	private float panelWidth;
	private int panelheight = 1;
	private List<String> titleCharacters;

	[SerializeField]
	private List<GameObject> targetCharacters;

	private Text m_textModelTitle;
	private Text m_textModelVersion;
	private Text m_textModelAuthor;
	private Text m_textModelContact;
	private Text m_textModelReference;
	private RawImage m_thumbnail;
	private Text m_textPermissionAllowed;
	private Text m_textPermissionViolent;
	private Text m_textPermissionSexual;
	private Text m_textPermissionCommercial;
	private Text m_textPermissionOther;
	private Text m_textDistributionLicense;
	private Text m_textDistributionOther;

	private Text AddLabelFormat(string label, int targetCanvas = 0)
	{
        RectTransform rt = DebugUIBuilder.instance.AddLabel(label, targetCanvas);
        rt.sizeDelta = new Vector2 (panelWidth, rt.rect.height * panelheight);
        Text text = rt.GetComponent<Text>();
        text.alignment = TextAnchor.MiddleLeft;
        text.resizeTextForBestFit = false;
		return text;
	}

	void Start ()
    {
        //Center Panel
    	panelWidth = 600.0f;
        selectText = AddLabelFormat("-");
        //Characterの数を調べてメニューに追加する targetCharactersから情報を持ってくる
        titleCharacters = new List<String>();
		for (int i = 0; i < targetCharacters.Count; ++i)
		{
			var component = targetCharacters[i].GetComponent<VRMMeta>();
			String title;
			if (component == null)
			{
				title = targetCharacters[i].name;
			}
			else
			{
				title = component.Meta.Title;
			}
			titleCharacters.Add(title);
	        DebugUIBuilder.instance.AddRadio(title, "group", delegate(Toggle t) { RadioPressed(title, "group", t); }) ;
		}
        DebugUIBuilder.instance.AddButton("Select", SelectButtonPressed);

        //Right Panel
    	panelWidth = 1800.0f;
        AddLabelFormat("モデル情報(Model Information)",1);
        m_textModelTitle = AddLabelFormat("-", 1);
        m_textModelVersion = AddLabelFormat("-", 1);
        m_textModelAuthor = AddLabelFormat("-", 1);
        m_textModelContact = AddLabelFormat("-", 1);
        m_textModelReference = AddLabelFormat("-", 1);
        AddLabelFormat("使用許諾・ライセンス情報(License)",1);
        panelheight = 2;
        m_textPermissionAllowed = AddLabelFormat("-", 1);
        m_textPermissionViolent = AddLabelFormat("-", 1);
        m_textPermissionSexual = AddLabelFormat("-", 1);
        panelheight = 1;
        m_textPermissionCommercial = AddLabelFormat("-", 1);
        m_textPermissionOther = AddLabelFormat("-", 1);
        panelheight = 2;
        AddLabelFormat("再配布・改変に関する許諾範囲(Redistribution / Modifications License)",1);
        panelheight = 1;
        m_textDistributionLicense = AddLabelFormat("-", 1);
        m_textDistributionOther = AddLabelFormat("-", 1);

        selectCharacter = 0;
        currentCharacter = 0;
        Debug.Log("targetCharacters.Count : "+targetCharacters.Count.ToString());
		if (targetCharacters.Count > 0)
		{
	        updateCharacterInfo(targetCharacters[selectCharacter]);
	        changeSetting(targetCharacters[selectCharacter]);
	        targetCharacters[selectCharacter].SetActive(true);
	        selectText.text = titleCharacters[selectCharacter] + " Selected";
		}
		else
		{
	        selectText.text = "No Character";
		}

        DebugUIBuilder.instance.Show();
        inMenu = true;
	}

    private void updateCharacterInfo(GameObject newCharacter)
    {
		if (newCharacter == null)
		{
	        Debug.Log("newCharacter null!!");
            return;
		}
		
		m_textModelTitle.text = "タイトル(Title):";
		m_textModelVersion.text = "バージョン(Version):";
		m_textModelAuthor.text =  "作者(Author):";
		m_textModelContact.text =  "連絡先(Contact Information):";
		m_textModelReference.text =  "参照(Reference):";
		
		m_textPermissionAllowed.text =  "アバターに人格を与えることの許諾範囲(A person who can perform with this avatar):";
		m_textPermissionViolent.text =  "このアバターを用いて暴力表現を演じることの許可(Violent acts using this avatar):";
		m_textPermissionSexual.text =  "このアバターを用いて性的表現を演じることの許可(Sexuality acts using this avatar):";
		m_textPermissionCommercial.text =  "商用利用の許可(For commercial use):";
		m_textPermissionOther.text =  "その他のライセンス条件(Other License Url):";
		
		m_textDistributionLicense.text =  "ライセンスタイプ(License Type):";
		m_textDistributionOther.text =  "その他ライセンス条件(Other License Url):";
		
		var component = newCharacter.GetComponent<VRMMeta>();
		if (component == null)
		{
			m_textModelTitle.text += "MazeCharacter";
			//m_thumbnail.texture = null;
			TitleText = "MazeCharacter";
		}
		else
		{
			var meta = component.Meta;
			m_textModelTitle.text += meta.Title;
			m_textModelVersion.text += meta.Version;
			m_textModelAuthor.text += meta.Author;
			m_textModelContact.text += meta.ContactInformation;
			m_textModelReference.text += meta.Reference;
			
			m_textPermissionAllowed.text += meta.AllowedUser.ToString();
			m_textPermissionViolent.text += meta.ViolentUssage.ToString();
			m_textPermissionSexual.text += meta.SexualUssage.ToString();
			m_textPermissionCommercial.text += meta.CommercialUssage.ToString();
			m_textPermissionOther.text += meta.OtherPermissionUrl;
			
			m_textDistributionLicense.text += meta.LicenseType.ToString();
			m_textDistributionOther.text += meta.OtherLicenseUrl;
			
			//m_thumbnail.texture = meta.Thumbnail;
			TitleText = meta.Title;
		}
    }

    private void RadioPressed(string radioLabel, string group, Toggle t)
    {
        //右パネルの表示を切り替える
		for (int i = 0; i < titleCharacters.Count; ++i)
		{
			if (titleCharacters[i] == radioLabel)
			{
				selectCharacter = i;
				break;
			}
		}
        //Characterの情報を表示する
	    updateCharacterInfo(targetCharacters[selectCharacter]);
    }

    private void SelectButtonPressed()
    {
        changeCharacter(selectCharacter);
    }

    private void changeCharacter(int nextCharacter)
    {
        //Characterを切り替える
        if (nextCharacter == currentCharacter)
        {
            return;
        }
        targetCharacters[currentCharacter].SetActive(false);
        //TODO 消えるエフェクト
        //Characterの位置を引き継ぐ
        targetCharacters[nextCharacter].transform.position = targetCharacters[currentCharacter].transform.position;
        targetCharacters[nextCharacter].transform.rotation = targetCharacters[currentCharacter].transform.rotation;
        targetCharacters[nextCharacter].GetComponent<NavMeshAgent>().Warp(targetCharacters[currentCharacter].transform.position);
        targetCharacters[nextCharacter].GetComponent<AICharacterControl>().SetTarget(targetCharacters[currentCharacter].transform.position);
        //Characterの参照先を切り替え
        changeSetting(targetCharacters[nextCharacter]);
        targetCharacters[nextCharacter].SetActive(true);
       //TODO 出現エフェクト
        currentCharacter = nextCharacter;
        selectText.text = TitleText + " Selected";
    }

    private void changeSetting(GameObject nextCharacter)
    {
        //Characterの参照先を切り替える
        //ExitMarker-ExitArea-m_PlayerTransform
        GameObject targetObject = GameObject.Find("ExitMarker");
        targetObject.GetComponent<ExitArea>().changeCharacter(nextCharacter);
        //MazeTurret-Turret-m_PlayerTransform
        //MazeTurret-Turret-m_Player
        targetObject = GameObject.Find("MazeTurret");
        targetObject.GetComponent<Turret>().changeCharacter(nextCharacter);
        //MazeSwitch-SwitchButton-m_Character
        targetObject = GameObject.Find("MazeSwitch");
        targetObject.GetComponent<SwitchButton>().changeCharacter(nextCharacter);
        //MazeGameController-MazeGameController-m_Player
        targetObject = GameObject.Find("MazeGameController");
        targetObject.GetComponent<MazeGameController>().changeCharacter(nextCharacter);
        //MazeAgentTrailGUI-AgentTrail-m_Agent
        targetObject = GameObject.Find("MazeAgentTrailGUI");
        targetObject.GetComponent<AgentTrail>().changeCharacter(nextCharacter);
    }

    void Update()
    {
        if(OVRInput.GetDown(OVRInput.Button.Two) || OVRInput.GetDown(OVRInput.Button.Start))
        {
            if (inMenu) DebugUIBuilder.instance.Hide();
            else DebugUIBuilder.instance.Show();
            inMenu = !inMenu;
        }
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
			selectCharacter++;
	        // changeCharacter
			if (selectCharacter >= titleCharacters.Count)
			{
				selectCharacter = 0;
			}
	        changeCharacter(selectCharacter);
        }
#endif
    }
}
