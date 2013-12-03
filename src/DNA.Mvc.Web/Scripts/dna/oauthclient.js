/*
** Project : TaoUI
** Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
** Licensed under the GPLv2: http://dotnetage.codeplex.com/license
** Project owner : Ray Liang (csharp2002@hotmail.com)
*/

oauthProxy = function (provider, authUrl, apiUrl) {
    this.provider = provider;
    var id = provider + "_service";
    window[id] = this;
    this.serverProxy = false;

    if (authUrl)
        this.authUrl = authUrl;
    else
        this.authUrl = "/oauth/authorize/";

    if (apiUrl)
        this.apiUrl = apiUrl;
    else
        this.apiUrl = "/api/oauth/call/";
}

oauthProxy.prototype = {
    saveToken: function (accessToken) {
        ///This method is call by Callback page.
        var _scrope = this._scope ? this._scope : "default";
        var token = {
            key: encodeURIComponent(this.provider + _scrope),
            provider: this.provider,
            scope: _scrope,
            accessToken: accessToken
        };
        this.token = token;
        if (this._deffered) {
            this._deffered.resolve(accessToken);
        }
    },
    authorize: function (scope) {
        var dfd = $.Deferred(), itemKey = encodeURIComponent(this.provider + (scope ? scope : "default"));

        //Check token whether it's initialized
        if (this.token) {
            if (this.token.key == itemKey) {
                return dfd.resolve(this.token.accessToken);
            }
        }

        //Get token from server
        var requestUrl = this.authUrl + this.provider + (scope ? ("?scope=" + encodeURIComponent(scope)) : "");
        console.log(requestUrl);
        window.open(requestUrl, "_blank", "height=500, width=800,toolbar=no,top=200,left=300, menubar=no, scrollbars=no, resizable=no,location=no, status=no");
        this._deffered = dfd;
        this._scope = scope;
        return dfd;

    },
    api: function (options, callback) {
        ///<example>
        ///        oauth2.api({
        ///            scope: "Required scope",
        ///            url: "REST URL",
        ///            type: "GET"
        ///        }, function (reponse) {
        ///            alert("The remote REST call is success" + response);
        ///        });
        ///</example>
        this._deferred = null;
        var self = this;
        this.authorize(options.scope)
               .done(function (accessToken) {
                   if (self.serverProxy) {
                       var _data = {
                           endPoint: options.url,
                           postData: options.data ? JSON.stringify(options.data) : "",
                           scope: options.scope
                       };

                       $.ajax({
                           url: self.apiUrl + self.provider,
                           type: options.type ? options.type : "GET",
                           cache: false,
                           data: _data
                       })
                           .done(function (reponse) {
                               if ($.isFunction(callback))
                                   callback(reponse);
                           }).fail(function () {
                               $.err(this.responseText);
                           });
                   } else {
                       var _postData = { access_token: accessToken };
                       if (options.data)
                           $.extend(_postData, options.data);

                       $.ajax({
                           url: options.url,
                           type: options.type ? options.type : "GET",
                           dataType: "jsonp",
                           cache: false,
                           data: _postData
                       }).done(function (reponse) {
                           if ($.isFunction(callback))
                               callback(reponse);
                       }).fail(function (response) {
                           $.err(this.responseText);
                       });
                   };
               });

    },
    convert: function (_provider, userInfo) {
        var _profile = { appName: _provider };

        _profile.locale = userInfo.locale ? userInfo.locale : "";

        if (_provider == "windowslive") {
            $.extend(_profile, {
                email: userInfo.emails.account,
                gender: userInfo.gender,
                displayName: encodeURIComponent(userInfo.name),
                account: userInfo.id,
                link: userInfo.link,
                firstName: userInfo.first_name,
                lastName:userInfo.last_name
            });
        }

        if (_provider == "google") {
            $.extend(_profile, {
                email: userInfo.email,
                gender: userInfo.gender,
                displayName: userInfo.name,
                firstName: userInfo.family_name,
                lastName:userInfo.given_name,
                account: userInfo.id,
                avatar: userInfo.picture ? userInfo.picture : "",
                link: userInfo.link
            });
        }

        if (_provider == "facebook") {
            $.extend(_profile, {
                email: userInfo.email,
                gender: userInfo.gender,
                displayName: userInfo.name,
                firstName: userInfo.first_name,
                lastName:userInfo.last_name,
                account: userInfo.username,
                signature: userInfo.bio,
                avatar: userInfo.picture ? userInfo.picture : "",
                link: userInfo.link
            });
        }

        if (_provider == "twitter") {
            $.extend(_profile, {
                displayName: userInfo.screen_name,
                account: userInfo.id,
                signature: userInfo.description,
                avatar: userInfo.profile_image_url ? userInfo.profile_image_url : "",
                link: "http://www.twitter.com/" + userInfo.screen_name
            });
        }

        if (_provider == "tumblr") {
            $.extend(_profile, {
                account: userInfo.response.user.name,
                displayName: userInfo.response.user.name
            });
            var _blogs = [];
            if (userInfo.response.blogs) {
                _blogs = userInfo.response.blogs;
                var _primaryBlog = null;
                $.each(_blogs, function (i, blog) {
                    if (blog.primary)
                        _primaryBlog = blog;
                });

                if (_primaryBlog) {
                    if (_primaryBlog.title)
                        _profile.displayName = _primaryBlog.title;

                    if (_primaryBlog.url)
                        _profile.link = _primaryBlog.url;
                }
            }
        }

        return _profile;
    }
}