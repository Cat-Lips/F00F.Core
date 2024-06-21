using Godot;
using Godot.Collections;
using HttpResponseCode = Godot.HttpClient.ResponseCode;
using HttpResult = Godot.HttpRequest.Result;
using HttpStatus = Godot.HttpClient.Status;

namespace F00F
{
    public partial class ProgressRequest : HttpRequest
    {
        private HttpStatus _Status;
        private int _BodySize;
        private int _DownloadBytes;

        private bool Changed { get; set; }
        private void ValueChanged() => Changed = true;

        public HttpStatus Status { get => _Status; private set => this.Set(ref _Status, value, ValueChanged); }
        public int BodySize { get => _BodySize; private set => this.Set(ref _BodySize, value, ValueChanged); }
        public int DownloadBytes { get => _DownloadBytes; private set => this.Set(ref _DownloadBytes, value, ValueChanged); }

        public event HttpProgress Progress;
        public event HttpResponse Response;

        public override void _Ready()
        {
            Status = GetHttpClientStatus();
            BodySize = GetBodySize();
            DownloadBytes = GetDownloadedBytes();
            RequestCompleted += OnRequestCompleted;
            Changed = false;

            void OnRequestCompleted(long _result, long _responseCode, string[] headers, byte[] body)
            {
                var result = (HttpResult)_result;
                var response = (HttpResponseCode)_responseCode;
                var data = GetResponse();

                Response?.Invoke(result, response, data);

                Dictionary GetResponse()
                {
                    var json = new Json();
                    json.Parse(body.GetStringFromUtf8());
                    return json.Data.AsGodotDictionary();
                }
            }
        }

        public override void _Process(double _)
        {
            Status = GetHttpClientStatus();
            BodySize = GetBodySize();
            DownloadBytes = GetDownloadedBytes();

            if (Changed)
            {
                Changed = false;
                Progress?.Invoke(Status, BodySize, DownloadBytes);
            }
        }
    }
}
