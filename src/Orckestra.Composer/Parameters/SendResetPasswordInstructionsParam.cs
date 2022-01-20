namespace Orckestra.Composer.Parameters
{
    public class SendResetPasswordInstructionsParam
    {
        public string Email { get; set; }
        public string Scope { get; set; }

        public SendResetPasswordInstructionsParam Clone()
        {
            var param = (SendResetPasswordInstructionsParam)MemberwiseClone();
            return param;
        }
    }
}
