﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using BackgroundMediaShared;
using Tomato.Media;
using Tomato.Media.Codec;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace HelloWorld
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private BackgroundMediaPlayerClient playerClient;
        private VideoPresenter videoPresenter;
        private const string fileName = "ms-appx:///Assets/04.花篝り.APE";//"ms-appx:///Assets/Ore no Imouto ga Konna ni Kawaii Wake ga Nai Opening.avi";

        public MainPage()
        {
            this.InitializeComponent();

            //Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            playerClient = new BackgroundMediaPlayerClient(typeof(BackgroundMediaPlayerHandler));
            playerClient.MessageReceived += PlayerClient_MessageReceived; ;

            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(fileName));
            var stream = await file.OpenReadAsync();
            var mediaSource = await MediaSource.CreateFromStream(stream, fileName);
            videoPresenter = new VideoPresenter();
            videoPresenter.SetMediaSource(mediaSource);
            img_Video.Source = videoPresenter.ImageSource;

            videoPresenter.Play();
        }

        private void PlayerClient_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (e.Message == "Ready to Play")
                playerClient.SendMessage("M", "Play");
            Debug.WriteLine($"Player Message: {e.Tag}, {e.Message}");
        }

        private void MediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }

        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {

        }
    }
}
