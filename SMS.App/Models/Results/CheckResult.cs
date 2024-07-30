using SMS.App.Models.Entities;

namespace SMS.App.Models.Results
{
    public class CheckResult()
    {
        public Target Target { get; set; } = new Target();
        public string ResultMessage {  get; set; } = "No message";
        public bool IsSuccess {  get; set; } = false;

        public override int GetHashCode()
        {
            return Target.URL.GetHashCode();
        }
    };

    public static class ChechResultExtensions
    {
        public static CheckResult SetTarget(this CheckResult result, Target target)
        {
            result.Target = target;
            return result;
        }

        public static CheckResult SetResultMessage(this CheckResult result, string resultMessage)
        {
            result.ResultMessage = resultMessage;
            return result;
        }

        public static CheckResult SetSuccess(this CheckResult result, bool isSuccess = true)
        {
            result.IsSuccess = isSuccess;
            return result;
        }
    }

}
