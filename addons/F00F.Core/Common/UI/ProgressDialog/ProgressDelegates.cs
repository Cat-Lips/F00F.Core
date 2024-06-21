using Godot.Collections;
using HttpResponseCode = Godot.HttpClient.ResponseCode;
using HttpResult = Godot.HttpRequest.Result;
using HttpStatus = Godot.HttpClient.Status;

namespace F00F
{
    public delegate void SetProgress(Status status, string msg = null, float? progress = null);
    public delegate void HttpProgress(HttpStatus status, int bodySize, int downloadBytes);
    public delegate void HttpResponse(HttpResult result, HttpResponseCode response, Dictionary data);
}
