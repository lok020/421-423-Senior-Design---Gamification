using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class BulletinBoardUI : MonoBehaviour {

    //Reference to the network manager, where all text will be stored
    private NetworkManager _network;

    //Set these from the editor - this is where all text will be stored
    //The string at index 0 is the header, 1 is page 2, 2 is page 2, etc.
    public List<string> TeacherAnnouncements;
    public List<string> PatchNotes;
    public List<string> UpcomingFeatures;
    public List<string> PageOne;
    public List<string> PageTwo;
    public List<string> PageThree;

    //These sprites have to be set so the script knows what images to update to
    public Sprite TeacherAnnouncementsFolded, TeacherAnnouncementsUnfolded;
    public Sprite PatchNotesFolded, PatchNotesUnfolded;
    public Sprite UpcomingFeaturesFolded, UpcomingFeaturesUnfolded;
    public Sprite PageOneFolded, PageOneUnfolded;
    public Sprite PageTwoFolded, PageTwoUnfolded;
    public Sprite PageThreeFolded, PageThreeUnfolded;

    //References to the various images and text objects
    private Image _teacherImage;
    private Image _patchImage;
    private Image _upcomingImage;
    private Image _pageOneImage;
    private Image _pageTwoImage;
    private Image _pageThreeImage;
    private Image _progressBar;

    private Text _teacherText, _teacherHeader;
    private Text _patchText, _patchHeader;
    private Text _upcomingText, _upcomingHeader;
    private Text _pageOneText, _pageOneHeader;
    private Text _pageTwoText, _pageTwoHeader;
    private Text _pageThreeText, _pageThreeHeader;
    private Text _progressBarTitle;
    
    //List of current page numbers
    private List<int> _pageNumbers = new List<int>();

    //Enum to make things easier to use
    private enum Page { TEACHER_ANNOUNCEMENTS, PATCH_NOTES, UPCOMING_FEATURES, PAGE_ONE, PAGE_TWO, PAGE_THREE }

	// Use this for initialization
	public void Start () {
        //First get references to each of the components
	    foreach(Image i in GetComponentsInChildren<Image>(true))
        {
            switch (i.name)
            {
                case "Teacher":
                    _teacherImage = i;
                    foreach (Text t in i.GetComponentsInChildren<Text>(true))
                    {
                        if (t.name == "Header") _teacherHeader = t;
                        else if (t.name == "Text") _teacherText = t;
                    }
                    break;
                case "Patchnotes":
                    _patchImage = i;
                    foreach (Text t in i.GetComponentsInChildren<Text>(true))
                    {
                        if (t.name == "Header") _patchHeader = t;
                        else if (t.name == "Text") _patchText = t;
                    }
                    break;
                case "Upcoming":
                    _upcomingImage = i;
                    foreach (Text t in i.GetComponentsInChildren<Text>(true))
                    {
                        if (t.name == "Header") _upcomingHeader = t;
                        else if (t.name == "Text") _upcomingText = t;
                    }
                    break;
                case "Page1":
                    _pageOneImage = i;
                    foreach (Text t in i.GetComponentsInChildren<Text>(true))
                    {
                        if (t.name == "Header") _pageOneHeader = t;
                        else if (t.name == "Text") _pageOneText = t;
                    }
                    break;
                case "Page2":
                    _pageTwoImage = i;
                    foreach (Text t in i.GetComponentsInChildren<Text>(true))
                    {
                        if (t.name == "Header") _pageTwoHeader = t;
                        else if (t.name == "Text") _pageTwoText = t;
                    }
                    break;
                case "Page3":
                    _pageThreeImage = i;
                    foreach (Text t in i.GetComponentsInChildren<Text>(true))
                    {
                        if (t.name == "Header") _pageThreeHeader = t;
                        else if (t.name == "Text") _pageThreeText = t;
                    }
                    break;
                case "ProgressBar":
                    _progressBarTitle = i.GetComponentsInChildren<Text>(true)[0];
                    break;
                case "ProgressBar_Bar":
                    _progressBar = i;
                    break;
            }
        }
        //Set up page number array
        for(int i =  0; i < System.Enum.GetNames(typeof(Page)).Length; i++)
        {
            _pageNumbers.Add(1);
        }
        //Get network manager
        _network = GameObject.Find("DatabaseManager").GetComponent<NetworkManager>();
        //Set List<string> page variables to what is stored in the network manager
        TeacherAnnouncements = _network.TB_Pages[0];
        PatchNotes = _network.TB_Pages[1];
        UpcomingFeatures = _network.TB_Pages[2];
        PageOne = _network.TB_Pages[3];
        PageTwo = _network.TB_Pages[4];
        PageThree = _network.TB_Pages[5];
        //Now set the text for each element
        SetPage(Page.TEACHER_ANNOUNCEMENTS, 1);
        SetPage(Page.PATCH_NOTES, 1);
        SetPage(Page.UPCOMING_FEATURES, 1);
        SetPage(Page.PAGE_ONE, 1);
        SetPage(Page.PAGE_TWO, 1);
        SetPage(Page.PAGE_THREE, 1);
        //Set progress bar
        RenderProgressBar();
        _progressBarTitle.text = _network.TB_ProgressTitle;
    }

    //Sets page text. If first page, displays header as well (if one is defined). Shows folded corner if not the last page
    private void SetPage(Page page, int pageNumber)
    {
        List<string> list = ListFromEnum(page);
        Image img = _pageThreeImage;
        Sprite folded = PageThreeFolded;
        Sprite unfolded = PageThreeUnfolded;
        Text header = _pageThreeHeader;
        Text text = _pageThreeText;
        int headerIndent = 50;
        switch (page) {
            case Page.TEACHER_ANNOUNCEMENTS:
                img = _teacherImage;
                header = _teacherHeader;
                text = _teacherText;
                folded = TeacherAnnouncementsFolded;
                unfolded = TeacherAnnouncementsUnfolded;
                headerIndent = 90;
                break;
            case Page.PATCH_NOTES:
                img = _patchImage;
                header = _patchHeader;
                text = _patchText;
                folded = PatchNotesFolded;
                unfolded = PatchNotesUnfolded;
                break;
            case Page.UPCOMING_FEATURES:
                img = _upcomingImage;
                header = _upcomingHeader;
                text = _upcomingText;
                folded = UpcomingFeaturesFolded;
                unfolded = UpcomingFeaturesUnfolded;
                break;
            case Page.PAGE_ONE:
                img = _pageOneImage;
                header = _pageOneHeader;
                text = _pageOneText;
                folded = PageOneFolded;
                unfolded = PageOneUnfolded;
                break;
            case Page.PAGE_TWO:
                img = _pageTwoImage;
                header = _pageTwoHeader;
                text = _pageTwoText;
                folded = PageTwoFolded;
                unfolded = PageTwoUnfolded;
                break;
            case Page.PAGE_THREE:
                img = _pageThreeImage;
                header = _pageThreeHeader;
                text = _pageThreeText;
                folded = PageThreeFolded;
                unfolded = PageThreeUnfolded;
                break;
        }

        //If this is blank for some reason, skip it
        if (list.Count == 0) return;

        //Show header and indent text if first page and header is defined
        StringBuilder sb = new StringBuilder();
        if (pageNumber == 1 && !string.IsNullOrEmpty(list[0]))
        {
            header.enabled = true;
            header.text = list[0];
            text.rectTransform.offsetMax = new Vector2(text.rectTransform.offsetMax.x, -headerIndent);
        }
        else
        {
            header.enabled = false;
            text.rectTransform.offsetMax = new Vector2(text.rectTransform.offsetMax.x, -15);
        }
        sb.Append(list[pageNumber]);
        sb.Replace("\\n", "\n");
        text.text = sb.ToString();
        //Show correct page background
        if((list.Count - 1) > pageNumber)
        {
            img.sprite = folded;
        }
        else
        {
            img.sprite = unfolded;
        }
    }

    private Page StringToEnum(string name)
    {
        switch (name.ToLower())
        {
            case "teacher": return Page.TEACHER_ANNOUNCEMENTS;
            case "patchnotes": return Page.PATCH_NOTES;
            case "upcoming": return Page.UPCOMING_FEATURES;
            case "page1": return Page.PAGE_ONE;
            case "page2": return Page.PAGE_TWO;
            case "page3": return Page.PAGE_THREE;
        }
        //Default
        return Page.PAGE_THREE;
    }

    private List<string> ListFromEnum(Page page)
    {
        switch(page)
        {
            case Page.TEACHER_ANNOUNCEMENTS: return TeacherAnnouncements;
            case Page.PATCH_NOTES: return PatchNotes;
            case Page.UPCOMING_FEATURES: return UpcomingFeatures;
            case Page.PAGE_ONE: return PageOne;
            case Page.PAGE_TWO: return PageTwo;
            case Page.PAGE_THREE: return PageThree;
        }
        return PageThree;
    }

    public void FlipLeft(string page)
    {
        FlipPage(page, "left");
    }

    public void FlipRight(string page)
    {
        FlipPage(page, "right");
    }

    private void FlipPage(string page, string leftOrRight)
    {
        //Do nothing
        Page p = StringToEnum(page);
        if(leftOrRight.ToLower() == "left")
        {
            //Do not go before page 1
            if(_pageNumbers[(int)p] > 1)
            {
                _pageNumbers[(int)p]--;
            }
        }
        else
        {
            //Get page count
            int pageCount = ListFromEnum(p).Count - 1;
            //Do not go past the last page
            if(_pageNumbers[(int)p] < pageCount)
            {
                _pageNumbers[(int)p]++;
            }
        }
        //Update page
        SetPage(p, _pageNumbers[(int)p]);
    }

    private void RenderProgressBar()
    {
        _progressBar.fillAmount = (float)_network.TB_Progress;
    }
}
