using System;
using System.Diagnostics;
using Godot;
using Godot.Collections;
using HttpResponseCode = Godot.HttpClient.ResponseCode;
using HttpResult = Godot.HttpRequest.Result;
using HttpStatus = Godot.HttpClient.Status;

namespace F00F;

public static class RequestExtensions
{
    public static void Request(this ProgressRequest source, string url, SetProgress SetProgress, Action<Dictionary> onResponse)
    {
        source.OnReady(() =>
        {
            source.Request(url, OnProgress, OnResponse).Err("HTTP", url, OnError);

            void OnError(string error)
                => SetProgress(Status.Error, error);

            void OnProgress(HttpStatus status, int bodySize, int downloadBytes)
            {
                if (status.IsError())
                    SetProgress(Status.Error, status.Str());
                else
                    SetProgress(Status.Info, null, GetProgress());

                float? GetProgress()
                {
                    return
                        downloadBytes < 0 ? null :
                        bodySize <= 0 ? downloadBytes :
                        downloadBytes / (float)bodySize;
                }
            }

            void OnResponse(HttpResult result, HttpResponseCode response, Dictionary data)
            {
                if (result.IsError())
                {
                    Debug.Assert(data?.Count is null or 0);
                    SetProgress(Status.Error, result.Str());
                    return;
                }

                if (response.IsError())
                {
                    Debug.Assert(data?.Count is null or 0);
                    SetProgress(Status.Error, response.Str());
                    return;
                }

                onResponse(data);
            }
        });
    }

    private static Error Request(this ProgressRequest source, string url, HttpProgress onProgress, HttpResponse onResponse)
    {
        return source.Request(url).Ok(() =>
        {
            source.Progress += onProgress;
            source.Response += OnResponse;
        });

        void OnResponse(HttpResult result, HttpResponseCode response, Dictionary data)
        {
            source.Progress -= onProgress;
            source.Response -= OnResponse;
            onResponse(result, response, data);
        }
    }

    private static bool IsError(this HttpResponseCode source) => source is not HttpResponseCode.Ok;
    private static bool IsError(this HttpResult source) => source is not HttpResult.Success;
    private static bool IsError(this HttpStatus source) => source is
        HttpStatus.CantResolve or
        HttpStatus.CantConnect or
        HttpStatus.ConnectionError or
        HttpStatus.TlsHandshakeError;
}
