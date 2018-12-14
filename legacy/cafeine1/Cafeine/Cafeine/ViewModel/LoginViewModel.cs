﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cafeine.Design;
using Cafeine.Model;
using GalaSoft.MvvmLight.Views;
using GalaSoft.MvvmLight.Messaging;
using Windows.Storage;

namespace Cafeine.ViewModel {
    public class LoginViewModel : ViewModelBase {
        private RelayCommand _ShowSignIn;
        private readonly INavigationService _navigationService;
        private ICafeineNavigationService _Fnavigationservice;
        public LoginViewModel(INavigationService navigationService, ICafeineNavigationService Fnavigationservice) {
            _navigationService = navigationService;
            _Fnavigationservice = Fnavigationservice;
            AutoLogin();
            SetupCacheFolder();
        }

        public RelayCommand ShowSignInDialog {
            get {
                return _ShowSignIn
                    ?? (_ShowSignIn = new RelayCommand(
                    async () => {
                        //TODO : Combine sign in page with login page for better code
                        SignInDialog dialog = new SignInDialog();
                        await dialog.ShowAsync();
                        Logincredentials lo = new Logincredentials();
                        bool verify = await lo.logincredential(dialog.u, dialog.p, 1);
                        if (verify == true) {
                            await DataProvider.GrabUserDatatoOffline(1);
                            _navigationService.NavigateTo("HomePage");
                            _Fnavigationservice.NavigateTo("VirDir");
                        }
                    }));
            }
        }
        private async Task AutoLogin() {
            var autologin = new Logincredentials().getcredentialfromlocker(1);
            if (autologin != null) {
                autologin.RetrievePassword();

                //check if user changed the password 
                Logincredentials login = new Logincredentials();
                bool canweusethepassword = await login.logincredential(autologin.UserName, autologin.Password, 1);
                switch (canweusethepassword) {
                    case true:
                    await DataProvider.GrabUserDatatoOffline(1);
                    _navigationService.NavigateTo("HomePage");
                    _Fnavigationservice.NavigateTo("VirDir");
                    break;
                    case false: break;
                }
            }
        }

        private async Task SetupCacheFolder() {
            var ImageCacheFolder = await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("Images", CreationCollisionOption.OpenIfExists);
            Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.AddOrReplace("ImageCacheFolder", ImageCacheFolder);


        }
    }
}