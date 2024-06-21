using Godot;
using Godot.Collections;
using HttpResponseCode = Godot.HttpClient.ResponseCode;
using HttpResult = Godot.HttpRequest.Result;
using HttpStatus = Godot.HttpClient.Status;

namespace F00F;

public partial class ProgressRequest : HttpRequest
{
    private bool Changed { get; set; }
    private void ValueChanged() => Changed = true;

    public HttpStatus Status { get; private set => this.Set(ref field, value, ValueChanged); }
    public int BodySize { get; private set => this.Set(ref field, value, ValueChanged); }
    public int DownloadBytes { get; private set => this.Set(ref field, value, ValueChanged); }

    public event HttpProgress Progress;
    public event HttpResponse Response;

    public sealed override void _Ready()
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

    public sealed override void _Process(double _)
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
