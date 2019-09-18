using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafeine.ViewModel {
    public class SignInDialogViewModel : ViewModelBase {
        private string text;
        private bool isOkButtonEnabled;

        public string Text {
            get { return text; }
            set {
                if (Set(nameof(Text), ref text, value)) {
                    IsOkButtonEnabled = !string.IsNullOrEmpty(value);
                }
            }
        }
        public bool IsOkButtonEnabled {
            get { return isOkButtonEnabled; }
            private set { Set(nameof(IsOkButtonEnabled), ref isOkButtonEnabled, value); }
        }
    }
}

