/*
** Project : TaoUI
** Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
** Licensed under the GPLv2: http://dotnetage.codeplex.com/license
** Project owner : Ray Liang (csharp2002@hotmail.com)
*/

(function ($, window, document, undefined) {
    $.fn.design = function () {
        if ($.fn.layout)
            $("[data-role='layout']").layout();
    }

    $.fn.enableComments = function () { }

    $.fn.enableOAuth = function () {
        var _appPah = $("body").data("root"),
            authEndpoint = _appPah + "oauth/authorize/",
            apiEndPoint = _appPah + "api/oauth/call/",
            signupEndPoint = _appPah + "signup";

        $("[data-role='oauthlogin']", this).each(function (i, n) {
            var element = $(n), _provider = element.data("provider"),
                _ver = element.data("version"),
                _userinfoEndPoint = element.data("profile-endpoint");

            element.click(function () {
                if (!window[_provider]) {
                    var oauth = new oauthProxy(_provider, authEndpoint, apiEndPoint),
                    _returnUrl = $(this).data("return");

                    if (_ver == "1.0a")
                        oauth.serverProxy = true;

                    oauth.api({ url: _userinfoEndPoint }, function (userInfo) {
                        if (userInfo.error) {
                            if (userInfo.error.message)
                                $.err("Login fail " + userInfo.error.message);
                            else {
                                if ($.type(userInfo.error) == "string")
                                    $.err("Login fail " + userInfo.error);
                            }
                        }
                        else {
                            var _profile = oauth.convert(_provider, userInfo);
                            //$("body").blockUI();
                            $.loading();
                            $.ajax({
                                url: _returnUrl ? signupEndPoint + "?returnUrl=" + encodeURIComponent(_returnUrl) : signupEndPoint,
                                data: _profile,
                                type: "POST"
                            }).done(function (data) {
                                //$("body").unblockUI();
                                $.loading("hide");
                                var _panel = $("<div/>").appendTo("body"),
                                    _body = $("<div/>").appendTo(_panel),
                                    _header = $("<h3/>").appendTo(_panel);
                                _body.append(data);

                                if ($("form", dlg).length && $.validator) {
                                    if ($.validator.unobtrusive) {
                                        if ($.isFunction($.validator.unobtrusive.parse)) {
                                            $.validator.unobtrusive.parse(dlg);
                                        }
                                    }
                                }

                                _body.taoUI();
                                _body.taoPanel({
                                    display: "dialog",
                                    opened: true
                                });

                                //dlg.dialog({
                                //    title: "Connect account",
                                //    autoOpen: true,
                                //    modal: true,
                                //    width: 650,
                                //    close: function () { dlg.remove(); }
                                //});

                            });
                        }
                    });
                }
            });
        });
    }

    $.fn.subscriber = function () {
        $("[data-role='subscriber']", this).each(function (i, n) {
            var _subscriber = $(n),
                _subscribedImg = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAACwBpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+Cjx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDQuMi4yLWMwNjMgNTMuMzUyNjI0LCAyMDA4LzA3LzMwLTE4OjEyOjE4ICAgICAgICAiPgogPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4KICA8cmRmOkRlc2NyaXB0aW9uIHJkZjphYm91dD0iIgogICAgeG1sbnM6eG1wPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvIgogICAgeG1sbnM6ZGM9Imh0dHA6Ly9wdXJsLm9yZy9kYy9lbGVtZW50cy8xLjEvIgogICAgeG1sbnM6cGhvdG9zaG9wPSJodHRwOi8vbnMuYWRvYmUuY29tL3Bob3Rvc2hvcC8xLjAvIgogICB4bXA6Q3JlYXRvclRvb2w9IkFkb2JlIFBob3Rvc2hvcCBDUzQgV2luZG93cyIKICAgeG1wOkNyZWF0ZURhdGU9IjIwMTItMDUtMTNUMDA6MDc6NDUrMDg6MDAiCiAgIHhtcDpNb2RpZnlEYXRlPSIyMDExLTAyLTA5VDA4OjMxOjA0KzA4OjAwIgogICB4bXA6TWV0YWRhdGFEYXRlPSIyMDExLTAyLTA5VDA4OjMxOjA0KzA4OjAwIgogICBkYzpmb3JtYXQ9ImltYWdlL3BuZyIKICAgcGhvdG9zaG9wOkNvbG9yTW9kZT0iMyIKICAgcGhvdG9zaG9wOklDQ1Byb2ZpbGU9InNSR0IgSUVDNjE5NjYtMi4xIi8+CiA8L3JkZjpSREY+CjwveDp4bXBtZXRhPgogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgCiAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAKICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgIAogICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAgICAg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",
                _unsubscribedImg = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAyJpVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuMC1jMDYwIDYxLjEzNDc3NywgMjAxMC8wMi8xMi0xNzozMjowMCAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RSZWY9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZVJlZiMiIHhtcDpDcmVhdG9yVG9vbD0iQWRvYmUgUGhvdG9zaG9wIENTNSBNYWNpbnRvc2giIHhtcE1NOkluc3RhbmNlSUQ9InhtcC5paWQ6MTRBMkE3QjMyQzc1MTFFMDg2RDRGNUM0ODM2NjgzNDIiIHhtcE1NOkRvY3VtZW50SUQ9InhtcC5kaWQ6MTRBMkE3QjQyQzc1MTFFMDg2RDRGNUM0ODM2NjgzNDIiPiA8eG1wTU06RGVyaXZlZEZyb20gc3RSZWY6aW5zdGFuY2VJRD0ieG1wLmlpZDoxNEEyQTdCMTJDNzUxMUUwODZENEY1QzQ4MzY2ODM0MiIgc3RSZWY6ZG9jdW1lbnRJRD0ieG1wLmRpZDoxNEEyQTdCMjJDNzUxMUUwODZENEY1QzQ4MzY2ODM0MiIvPiA8L3JkZjpEZXNjcmlwdGlvbj4gPC9yZGY6UkRGPiA8L3g6eG1wbWV0YT4gPD94cGFja2V0IGVuZD0iciI/PjciJMQAAAQySURBVHjatFe7ThtBFL37wMa8jXi/Hw4EiSZ0VHxFlC9IUqUIUppIEVG6hCpFRKKUSYvCFziio4GIAkERCSSwxcsIsHgZs5t7Bo+1jGfXa0RGGq9ndmbumTP3nrlrkGEQF+N3Mtna1dX1zrbtF9yO0MOXa9d10/l8/tvjsbFP3Ha4umzaMB6NjETW1ta+HxwcuP+zsHE3lUq5sMV2bdg2+Mf6s7LytrOz80NTU5OAymPF07hlp6So/XK8WnT9mMsbpcPDw+knExOfTe6zqqurn9fV1YkJjuMUJ+KpW0T2l3uvG4/1GxoaKBqNvuRXto0fri1AhgHq7sq1dTuWY7xtLyDLstDXC9uCAX5pApkf+jBFt2O//4VqcZcJAAaMe3cW5r/ubOV79b+uFDYsAJSc530Y8June3ptmR40gR6v84VK/MNbbm5uijYFgMvLS2NpaYm2t7eJYzVwB5UUdW42myXWAFpcXKRcLifQ2uLHtt3BwUFikaD19XViRaS+vj6SuhA2xv2OEHG/tbVFp6en1NvbS5OTk7Szs+MWAYCO1tZWamtro6urK7yk5eVlikQiAkh3dzdVVVVp6fZjBkxinc3NTWxQrDE+Pl5cRx6B7W1gURYlGhoaop6eHspkMrS3tydYYaWk/v5+am5u1hqW7bOzM2EUxuPxuFgLTGJdxL8UoxIAqlDU1NQItFAtUHd8fCxYQR9YAUAwJI2ztArDR0dHgs3R0VExt7a2VjCgRpmWAbXAGNBjIVnPz88FKxsbG9Te3k6NjY3CeVHATiKRKM4xOcr95PoOAISF38BbtTDFoqAUAE5OTkQbdO/u7oodgzG8r6+vD6WIsFlyBCoAVcNRYrGYMAaHxbFwCIud44zDyHDgEXgvI/UiUZ9gRTpkOd0oewR+DKigdO0gAEH/AwHojKCP1Qv3uBaULDgSb3SUA2CqYaijDo6HUJRerUtKZMUYjL24uAi8oEoYQDVvL8fiDrFjeDqyJdSgLEm+g4ZgLBQVQgaHVRnxFSKZF8AwLg54NkJLd8Z+UizbMAodwTpgBKC8QLRRAMMYjIkwLKVTF45hLyUYRuhiXVToBNa9A0BKqRQcSCeMSka8RoIuIb/zxhwo5vX1tdAO7zFKBhwgBeWqH5SjPAwI+R8ba2lpEU7NJovXscPIUnx9JqQ8lstqK0nN1Lb0N7aXwSts1+HL5QcSBgCQ6ZJfXh9k1O97wKv/yBPS6TSSlJ+4hpDYwTUbv87NfRkeHn6KxAHeGjYbroQBPPf392l1dXX+9fT0Kw7VDACAhRjX+Ozs7JvOjo5nDAAfKtZDf53i85DZnv+1sPAxmUz+hcbJ1BbGolxrCzVaUEnjvsZ4E+pch03lOATZB8+zUG1kbob8PC8YtAqOaVVinDVDO5ZD704/q2qeM6T8wMBAfmpqynk/M+P+E2AAVdI5RNT2ss0AAAAASUVORK5CYII=",
                _token = "", _appPath = "/";

            if ($(this).data("token"))
                _token = $(this).data("token");

            if ($(this).data("app"))
                _appPath = $(this).data("app");

            if ($(this).data("img-subscribed"))
                _subscribedImg = $(this).data("img-subscribed");

            if ($(this).data("img-unsubscribed"))
                _subscribedImg = $(this).data("img-unsubscribed");

            var element = $(this), _setStatus = function (flag) {
                if (flag)
                    element.empty().append($("<img/>").attr("title", "Unsubscribe").attr("src", _subscribedImg));
                else
                    element.empty().append($("<img/>").attr("title", "Subscribe").attr("src", _unsubscribedImg));
                element.data("subscribed", flag);
            }

            if (_token == "")
                _token = encodeURIComponent(document.URL);

            $.ajax({
                url: _appPath + "api/notify/status",
                data: { token: _token }
            })
              .done(function (flag) {
                  _setStatus(flag);
                  element.click(function (e) {
                      e.stopPropagation();
                      e.preventDefault();
                      $.ajax({
                          url: _appPath + "api/notify/subscribe",
                          type: "POST",
                          data: { token: _token }
                      }).done(function (returns) {
                          _setStatus(returns);
                      });
                  });
              });
        });
    }

})(jQuery, window, document);

$(function () {
    $("body").enableComments();
    $("body").subscriber();
});