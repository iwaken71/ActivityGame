﻿using Assets.eDriven.Demo;
using Assets.eDriven.Demo.Gui.Code;
using Assets.eDriven.Skins;
using eDriven.Animation;
using eDriven.Audio;
using eDriven.Core.Caching;
using eDriven.Gui;
using eDriven.Gui.Components;
using eDriven.Gui.Containers;
using eDriven.Gui.Events;
using eDriven.Gui.Layout;
using eDriven.Gui.Managers;
using eDriven.Gui.Plugins;
using eDriven.Gui.Stages;
using Assets.eDriven.Demo.Tweens;
using UnityEngine;
using Action = eDriven.Animation.Action;

public class DialogDemo2 : Gui
{
    #region Effects

    private readonly TweenFactory _windowShow = new TweenFactory(
        new Sequence(
            //new Action(delegate { Debug.Log("Running!"); }),
            new Action(delegate { AudioPlayerMapper.GetDefault().PlaySound("dialog_open"); }),
            new Jumpy()
        )
    );

    private readonly TweenFactory _overlayShow = new TweenFactory(
        new Sequence(
            new FadeIn()
        )
    );

    #endregion

    protected override void OnInitialize()
    {
        base.OnInitialize();

        //Alert.DefaultSkin = typeof(AlertSkin2);
        Dialog.AddedEffect = _windowShow;
        ModalOverlay.DefaultSkin = typeof(ModalOverlaySkin2);
        ModalOverlay.AddedEffect = _overlayShow;

        Layout = new VerticalLayout
        {
            PaddingLeft = 0,
            PaddingRight = 0,
            PaddingTop = 0,
            PaddingBottom = 0,
            Gap = 0
        };
    }

    private int _count = 0;

    override protected void CreateChildren()
    {
        base.CreateChildren();

        PrepareWindow();

        #region Controls

        Toolbar toolbar = new Toolbar();
        AddChild(toolbar);

        Button btnWindow = new Button
        {
            Text = "New window",
            FocusEnabled = false,
            SkinClass = typeof(ImageButtonSkin),
            Icon = ImageLoader.Instance.Load("Icons/comment")
        };
        btnWindow.Click += delegate
        {
            var window = GetWindow();
            if (null != window)
            {
                window.Visible = window.IncludeInLayout = true;
                window.SetStyle("addedEffect", _windowShow);
                //window.Plugins.Add(new Resizable { ShowOverlay = false });
                window.AddEventListener(CloseEvent.CLOSE, delegate { PopupManager.Instance.RemovePopup(window); });
                PopupManager.Instance.AddPopup(window, false);
                PopupManager.Instance.CenterPopUp(window);
                _windowShow.Callback = delegate
                {
                    PrepareWindow();
                };
            }
        };
        toolbar.AddContentChild(btnWindow);

        Button btnAlert = new Button
        {
            Text = "Alert",
            FocusEnabled = false,
            SkinClass = typeof(ImageButtonSkin),
            Icon = ImageLoader.Instance.Load("Icons/comment")
        };
        btnAlert.Click += delegate
        {
            Alert.DefaultSkin = null;
            Alert.Show(
                "Title", "Message", AlertButtonFlag.Ok,
                new AlertOption(AlertOptionType.Icon, Resources.Load<Texture>("edriven_gui")),
                new AlertOption(AlertOptionType.HeaderIcon, Resources.Load<Texture>("Icons/accept"))
            );
        };
        toolbar.AddContentChild(btnAlert);

        btnAlert = new Button
        {
            Text = "Alert (skin 2)",
            FocusEnabled = false,
            SkinClass = typeof(ImageButtonSkin),
            Icon = ImageLoader.Instance.Load("Icons/comment")
        };
        btnAlert.Click += delegate
        {
            Alert.DefaultSkin = typeof(AlertSkin2);
            Alert.Show(
                "Title", "Message", AlertButtonFlag.Ok,
                new AlertOption(AlertOptionType.Icon, Resources.Load<Texture>("edriven_gui")),
                new AlertOption(AlertOptionType.HeaderIcon, Resources.Load<Texture>("Icons/accept"))
            );
        };
        toolbar.AddContentChild(btnAlert);

        btnAlert = new Button
        {
            Text = "Alert (skin 3)",
            FocusEnabled = false,
            SkinClass = typeof(ImageButtonSkin),
            Icon = ImageLoader.Instance.Load("Icons/comment")
        };
        btnAlert.Click += delegate
        {
            Alert.DefaultSkin = typeof(AlertSkin3);
            Alert.Show(
                "Title", "Message", AlertButtonFlag.Ok,
                new AlertOption(AlertOptionType.Icon, Resources.Load<Texture>("edriven_gui")),
                new AlertOption(AlertOptionType.HeaderIcon, Resources.Load<Texture>("Icons/accept"))
            );
        };
        toolbar.AddContentChild(btnAlert);

        #endregion

        Scroller scroller = new Scroller
        {
            SkinClass = typeof(ScrollerSkin2),
            PercentWidth = 100,
            PercentHeight = 100
        };
        AddChild(scroller);

        Group viewport = new Group
        {
            Layout = new VerticalLayout
            {
                HorizontalAlign = HorizontalAlign.Left,
                PaddingLeft = 10,
                PaddingRight = 10,
                PaddingTop = 10,
                PaddingBottom = 10,
                Gap = 10
            }
        };
        scroller.Viewport = viewport;
    }

    private MyWindow _window;

    private MyWindow GetWindow()
    {
        /*DeferManager.Instance.Defer(delegate
        {
            var window = new MyWindow
            {
                Title = "The Window " + _count,
                Id = "window_" + _count,
                SkinClass = typeof(WindowSkin2),
                Icon = ImageLoader.Instance.Load("Icons/balloon_32"),
                Width = 400,
                Height = 600,
                Visible = false,
                IncludeInLayout = false
            };

            /*window.SetStyle("addedEffect", _windowShow);
            window.Plugins.Add(new Resizable { ShowOverlay = false });
            window.AddEventListener(CloseEvent.CLOSE, delegate { PopupManager.Instance.RemovePopup(window); });#1#

            PopupManagerStage.Instance.AddChild(window);

            _window = window;

        }, 5);*/

        if (null != _window)
        {
            PopupManagerStage.Instance.RemoveChild(_window);
            var window = _window;
            _window = null;
            return window;
        }
        PrepareWindow();
        return _window;
    }

    public void PrepareWindow()
    {
        _count++;

        Debug.Log("PrepareWindow");
        var window = new MyWindow
        {
            Title = "The Window " + _count,
            Id = "window_" + _count,
            SkinClass = typeof(WindowSkin2),
            Icon = ImageLoader.Instance.Load("Icons/balloon_32"),
            Width = 400,
            Height = 600,
            Visible = false,
            IncludeInLayout = false
        };
        window.Plugins.Add(new Resizable { ShowOverlay = false });
        PopupManagerStage.Instance.AddChild(window);
        _window = window;
    }
}