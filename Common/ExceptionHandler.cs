namespace G_APIs.Common
{
    public static class GoldExceptions
    {
        public static string LoginTypeError(LoginType k)
        {

            var err = new List<Dictionary<LoginType, string>>();
            err.Add(new Dictionary<LoginType, string>()
                {
                    { LoginType.Loggedin, "ورود موفق آمیز" },
                    { LoginType.Registed, "ثبت نام با موفقیت انجام شد." },
                    { LoginType.UserExist, "این کاربر قبلا ثبت شده است" },
                    { LoginType.UserNotExist, "کاربر وارد شده ثبت نشده است" },
                    { LoginType.Forbidden, "دسترسی غیر مجاز" },
                    { LoginType.WrongConfirmCode, "کد موبایل وارد شده اشتباه است" },
                    { LoginType.WrongUserOrPassword, "نام کاربری یا کلمه عبور اشتباه است" },
                    { LoginType.ConfirmCodeSendingError, "بروز خطا در ارسال پیامک. لطفا لحظاتی بعد تلاش کنید" },
                    { LoginType.ConfirmCodeAlreadySent, "کد برای شما ارسال شده است" },
                    { LoginType.PasswordHasChanged, " رمز عبور با موفقیت تغییر یافت" },

                });

            var dict = err.FirstOrDefault(d => d.ContainsKey(k));
            var message = "بروز خطا لطفا دقایقی بعد تلاش کنید.";
            dict?.TryGetValue(k, out message);
            return message;

        }
        public static string CroudTypeError(CroudType k)
        {

            var err = new List<Dictionary<CroudType, string>>();
            err.Add(new Dictionary<CroudType, string>()
                {
                    { CroudType.Successful, "عملیات با موفقیت انجام شد." },
                    { CroudType.Error, "بروز خطا   لطفا دقایقی بعد امتحان نمایید." },
                    { CroudType.ServerError, "بروز خطا   لطفا دقایقی بعد امتحان نمایید." },
                    { CroudType.TimeOut, "عدم ارتباط با سرور لطفا  دقایقی بعد تلاش نمایید.  " },

                });

            var dict = err.FirstOrDefault(d => d.ContainsKey(k));
            var message = "بروز خطا لطفا دقایقی بعد تلاش کنید.";
            dict?.TryGetValue(k, out message);
            return message;

        }
        public static JsonResult Throw(Exception ex)
        {
            return new JsonResult()
            { Data = new ApiResult(-1, ex.Message, ex.InnerException) };

        }
    }
}