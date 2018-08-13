using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breeze.FontSystem
{
    public static class MDL2SymbolsExtension
    {
        public static string AsChar(this MDL2Symbols symb)
        {
            return ((char)symb).ToString();
        }
    }
    public enum MDL2Symbols
    {
        Caret = '\uE933',
        Tape = '\uE96A',
        TickBox = 61804 ,
        Box = 61803,
        Finger = '\uf271',
        Lock = '\ue1F6',
        UnLock = '\ue1F7',
        Resize = '\uf0d8',
        BulletPoint = '\ue1F5',

        Accept = '\ue10b',

        Account = '\ue168',

        Add = '\ue109',

        AddFriend = '\ue1e2',

        Admin = '\ue1a7',

        AlignCenter = '\ue1a1',

        AlignLeft = '\ue1a2',

        AlignRight = '\ue1a0',

        AllApps = '\ue179',

        Attach = '\ue16c',

        AttachCamera = '\ue12d',

        Audio = '\ue189',

        Back = '\ue112',

        BackToWindow = '\ue1d8',

        BlockContact = '\ue1e0',

        Bold = '\ue19b',

        Bookmarks = '\ue12f',

        BrowsePhotos = '\ue155',

        Bullets = '\ue133',

        Calculator = '\ue1d0',

        Calendar = '\ue163',

        CalendarDay = '\ue161',

        CalendarReply = '\ue1db',

        CalendarWeek = '\ue162',

        Camera = '\ue114',

        Cancel = '\ue10a',

        Caption = '\ue15a',

        CellPhone = '\ue1c9',

        Character = '\ue164',

        Clear = '\ue106',

        ClearSelection = '\ue1c5',

        Clock = '\ue121',

        ClosedCaption = '\ue190',

        ClosePane = '\ue127',
        ClosePane2 = '\uef2c',
        ClosePaneMirrored = '\ueA49',
        Comment = '\ue134',

        Contact = '\ue13d',

        Contact2 = '\ue187',

        ContactInfo = '\ue136',

        ContactPresence = '\ue181',

        Copy = '\ue16f',

        Crop = '\ue123',

        Cut = '\ue16b',

        Delete = '\ue107',

        Directions = '\ue1d1',

        DisableUpdates = '\ue194',

        DisconnectDrive = '\ue17a',

        Dislike = '\ue19e',

        DockBottom = '\ue147',

        DockLeft = '\ue145',

        DockRight = '\ue146',

        Document = '\ue130',

        Download = '\ue118',

        Edit = '\ue104',

        Emoji = '\ue11d',

        Emoji2 = '\ue170',

        Favorite = '\ue113',

        Filter = '\ue16e',

        Find = '\ue11a',

        Flag = '\ue129',

        Folder = '\ue188',

        Font = '\ue185',

        FontColor = '\ue186',

        FontDecrease = '\ue1c6',

        FontIncrease = '\ue1c7',

        FontSize = '\ue1c8',

        Forward = '\ue111',

        FourBars = '\ue1e9',

        FullScreen = '\ue1d9',

        GlobalNavigationButton = '\ue700',

        Globe = '\ue12b',

        Go = '\ue143',

        GoToStart = '\ue1e4',

        GoToToday = '\ue184',

        HangUp = '\ue137',

        Help = '\ue11b',

        HideBcc = '\ue16a',

        Highlight = '\ue193',

        Home = '\ue10f',

        Import = '\ue150',

        ImportAll = '\ue151',

        Important = '\ue171',

        Italic = '\ue199',

        Keyboard = '\ue144',

        LeaveChat = '\ue11f',

        Library = '\ue1d3',

        Like = '\ue19f',

        LikeDislike = '\ue19d',

        Link = '\ue167',

        List = '\ue14c',

        Mail = '\ue119',

        MailFilled = '\ue135',

        MailForward = '\ue120',

        MailReply = '\ue172',

        MailReplyAll = '\ue165',

        Manage = '\ue178',

        Map = '\ue1c4',

        MapDrive = '\ue17b',

        MapPin = '\ue139',

        Memo = '\ue1d5',

        Message = '\ue15f',

        Microphone = '\ue1d6',

        More = '\ue10c',

        MousePointer = '\ue8b0',

        MoveToFolder = '\ue19c',

        MusicInfo = '\ue142',

        Mute = '\ue198',

        NewFolder = '\ue1da',

        NewWindow = '\ue17c',

        Next = '\ue101',

        OneBar = '\ue1e6',

        OpenFile = '\ue1a5',

        OpenLocal = '\ue197',

        OpenPane = '\ue126',

        OpenPaneMirrored = '\uea5B',

        OpenWith = '\ue17d',

        Orientation = '\ue14f',

        OtherUser = '\ue1a6',

        OutlineStar = '\ue1ce',

        Page = '\ue132',

        Page2 = '\ue160',

        Paste = '\ue16d',

        Pause = '\ue103',

        People = '\ue125',

        Permissions = '\ue192',

        Phone = '\ue13a',

        PhoneBook = '\ue1d4',

        Pictures = '\ue158',

        Pin = '\ue141',

        Placeholder = '\ue18a',

        Play = '\ue102',

        PostUpdate = '\ue1d7',

        Preview = '\ue295',

        PreviewLink = '\ue12a',

        Previous = '\ue100',

        Print = '\ue749',

        Priority = '\ue182',

        ProtectedDocument = '\ue131',

        Read = '\ue166',

        Redo = '\ue10d',

        Refresh = '\ue149',

        Remote = '\ue148',

        Relationship = '\uf003',

        Remove = '\ue108',

        Rename = '\ue13e',

        Repair = '\ue15e',

        RepeatAll = '\ue1cd',

        RepeatOne = '\ue1cc',

        ReportHacked = '\ue1de',

        ReShare = '\ue1ca',

        Rotate = '\ue14a',

        RotateCamera = '\ue124',

        Save = '\ue105',

        SaveLocal = '\ue159',

        Scan = '\ue294',

        SelectAll = '\ue14e',

        Send = '\ue122',

        SetLockScreen = '\ue18c',

        SetTile = '\ue18d',

        Setting = '\ue115',

        Share = '\ue72d',

        Shop = '\ue14d',

        ShowBcc = '\ue169',

        ShowResults = '\ue15c',

        Shuffle = '\ue14b',

        SlideShow = '\ue173',

        SolidStar = '\ue1cf',

        Sort = '\ue174',

        Stop = '\ue15b',

        StopSlideShow = '\ue191',

        Street = '\ue1c3',

        Switch = '\ue13c',

        SwitchApps = '\ue1e1',

        Sync = '\ue117',

        SyncFolder = '\ue1df',

        Tag = '\ue1cb',

        Target = '\ue1d2',

        ThreeBars = '\ue1e8',

        TouchPointer = '\ue1e3',

        Trim = '\ue12c',

        TwoBars = '\ue1e7',

        TwoPage = '\ue11e',

        Underline = '\ue19a',

        Undo = '\ue10e',

        UnFavorite = '\ue195',

        UnPin = '\ue196',

        UnSyncFolder = '\ue1dd',

        Up = '\ue110',

        Upload = '\ue11c',

        Video = '\ue116',

        VideoChat = '\ue138',

        View = '\ue18b',

        ViewAll = '\ue138',

        Volume = '\ue15d',

        WebCam = '\ue156',

        World = '\ue128',

        XboxOneConsole = '\ue990',

        ZeroBars = '\ue1e5',

        Zoom = '\ue1a3',

        ZoomIn = '\ue12e',

        ZoomOut = '\ue1a4'

    }
}
