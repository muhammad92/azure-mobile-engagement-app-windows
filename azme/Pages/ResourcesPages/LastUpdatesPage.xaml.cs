﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Azme.Models;
using Azme.Services;
using Azme.ViewModels;
using Microsoft.Azure.Engagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace Azme.Pages
{
    public sealed partial class LastUpdatesPage : EngagementPage
    {

        private LastUpdatesViewModel viewModel;

        public LastUpdatesPage()
        {
            this.InitializeComponent();
        }

        protected override string GetEngagementPageName()
        {
            return "recent_updates";
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ComputeViewModel();
        }

        private void ComputeViewModel()
        {
            viewModel = new LastUpdatesViewModel() { IsLoading = true };
            DataContext = viewModel;

            LoadFeedUpdate();
        }

        private async void LoadFeedUpdate()
        {
            viewModel.IsLoading = true;
            AzmeFeed feed = await AzmeServices.Instance.RequestRecentUpdates();
            if (feed != null && feed.Updates != null && feed.Updates.Count > 0)
            {
                viewModel.FeedUpdateList = feed.Updates;
            }
            viewModel.IsLoading = false;
        }
        private void OnItemClicked(object sender, SelectionChangedEventArgs e)
        {
            var entry = ListViewMenu.SelectedItem as AzmeFeed.FeedUpdate;
            if (entry != null)
            {
                var extras = new Dictionary<Object, Object>();
                extras[Constants.Parameters.URL] = entry.Link;
                extras[Constants.Parameters.TITLE] = entry.Title;
                EngagementAgent.Instance.SendEvent(Constants.Engagement.EVENT_CLICK_ARTICLE, extras);

                var parameters = new Dictionary<string, Object>();
                parameters[Constants.Parameters.URL] = entry.Link;
                Frame.Navigate(typeof(WebPage), parameters);
            }
        }

        private void ButtonRefresh_Tapped(object sender, TappedRoutedEventArgs e)
        {
            LoadFeedUpdate();
        }
    }
}
