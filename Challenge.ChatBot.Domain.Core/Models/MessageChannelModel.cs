namespace Challenge.ChatBot.Domain.Core.Models
{
    public class MessageChannelModel
    {
        public string Symbol { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Open { get; set; }
        public string High { get; set; }
        public string Low { get; set; }
        public string Close { get; set; }
        public string Volume { get; set; }
        public string Receiver { get; set; }
        public string MessageReturn
        {
            get
            {
                return !this.Close.Equals("N/D") ?
                                    $"{this.Symbol} quote is ${this.Close} per share" :
                                    "Error";
            }
        }
    }
}
