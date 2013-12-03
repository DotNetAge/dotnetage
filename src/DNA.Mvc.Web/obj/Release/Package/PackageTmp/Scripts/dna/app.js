/*
The DNA client service model for RESTful apis
*/
(function ($, window, document, undefined) {

    $DNA_Web = function () {
        this.name = $("body").data("web");
        this.lang = $("body").attr("lang");
        //this.categories = new Function();
        var self = this;
        this.cats = {
            load: function (parentID) {
                var dfd = $.Deferred();
                $.ajax($.resolveUrl("~/api/" + self.name + "/" + self.lang + "/cats/items/" + (parentID != undefined ? parentID : 0)))
                  .done(function (data) {
                      return dfd.resolve(data);
                  })
                  .fail(function (jqXHR, textStatus, errorThrown) {
                      $.err(errorThrown);
                      return dfd.reject();
                  });
                return dfd;
            },
            add: function (data) {
                var dfd = $.Deferred();
                $.post($.resolveUrl("~/api/" + self.name + "/" + self.lang + "/cats/add"), data)
                  .done(function (data) {
                      return dfd.resolve(data);
                  })
                  .fail(function (jqXHR, textStatus, errorThrown) {
                      $.err(errorThrown);
                      return dfd.reject();
                  });
                return dfd;
            },
            remove: function (id) {
                var dfd = $.Deferred();
                $.post($.resolveUrl("~/api/" + self.name + "/" + self.lang + "/cats/delete/" + id))
                  .done(function (data) {
                      return dfd.resolve(data);
                  })
                  .fail(function (jqXHR, textStatus, errorThrown) {
                      $.err(errorThrown);
                      return dfd.reject();
                  });
                return dfd;
            },
            move: function (id, parentID) {
                var dfd = $.Deferred();
                $.post($.resolveUrl("~/api/" + self.name + "/" + self.lang + "/cats/move/" + id), { parentID: parentID })
                  .done(function (data) {
                      return dfd.resolve(data);
                  })
                  .fail(function (jqXHR, textStatus, errorThrown) {
                      $.err(errorThrown);
                      return dfd.reject();
                  });
                return dfd;
            },
            update: function (data) {
                var dfd = $.Deferred();
                $.post($.resolveUrl("~/api/" + self.name + "/" + self.lang + "/cats/update"), data)
                  .done(function (data) {
                      return dfd.resolve(data);
                  })
                  .fail(function (jqXHR, textStatus, errorThrown) {
                      $.err(errorThrown);
                      return dfd.reject();
                  });
                return dfd;
            }
        };
    };

    $.extend($DNA_Web.prototype, {
        localizeTo: function (to) {
            ///<summary>Localize current website to target language</summary>
            if (to == undefined)
                $.err("app.web.localizeTo : The target language no set");

            if (to == this.lang) {
                $.notify("Current website already using " + to);
                return;
            }

            $.loading();
            var _defaultUrl = $.resolveUrl("~/dashboard/" + this.name + "/" + to + "/settings");
            return $.ajax({
                type: "POST",
                url: $.resolveUrl("~/api/" + this.name + "/langs/copy?locale=" + this.lang),
                data: { to: to },
                complete: function () { $.loading("hide"); },
                success: function () {
                    location = _defaultUrl;
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    $.err(errorThrown);
                }
            });
        },
        switchTo: function (to) {
            ///<summary>Localize current website to target language</summary>
            if (to == undefined)
                $.err("app.web.switchTo : The target language no set");

            if (to == this.lang) {
                $.notify("Current website already using " + to);
                return;
            }

            $.loading();
            return $.ajax({
                type: "POST",
                url: $.resolveUrl("~/api/" + this.name + "/langs/switch"),
                data: { to: to },
                complete: function () { $.loading("hide"); },
                success: function () {
                    location.reload();
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    $.err(errorThrown);
                }
            });
        }
    });

    $DNA_Page = function (parentWeb) {
        this.parent = parentWeb;
        this.id = $("body").data("id");
        this.title = $("title").text();
        this.description = $("meta[name=description]").attr("content");
        this.keywords = $("meta[name=keywords]").attr("content");
    }

    $.extend($DNA_Page.prototype, {
        getWidgets: function (callback) {
            var self = this;
            $.ajax({
                url: $.resolveUrl("~/api/" + this.parent.name + "/widgets?id=" + this.id)
            })
              .done(function (data) {
                  if ($.isFunction(callback))
                      callback(data);
                  self.widgets = data;
              });
            return this;
        },
        getZones: function () { },
        reset: function () {
            $.loading();
            return $.ajax({
                url: $.resolveUrl("~/api/" + this.parent.name + "/pages/reset/" + this.id),
                type: "post",
                data: { includeLayout: true },
                complete: function () { $.loading("hide"); }
            });
        },
        extractKeywords: function () {
            var tags = [];
            //1.heading
            $("h1,h2,h3,h4,h5,h6", $(".d-page")).each(function (i, heading) {
                var h = $(heading).text();
                if (h && $.inArray(h, tags) == -1)
                    tags.push($.trim(h.replace("\t", "").replace("\n", "")));
            });

            //2.titles
            $("[title]", $(".d-page")).each(function (i, ti) {
                var title = $(ti).attr("title");
                if (title && $.inArray(title, tags) == -1)
                    tags.push($.trim(title.replace("\t", "").replace("\n", "")));
            });
            //console.log(tags);
            return tags;
        }
    });

    $DNA_FUC_InstallPKG = function (type, id, name, category) {
        return $.ajax({
            url: $.resolveUrl("~/api/cloud/install/" + id),
            data: {
                type: type,
                name: name,
                category: category
            },
            error: function (jqXHR, textStatus, errorThrown) {
                $.err(errorThrown);
            }
        });
    };

    $DNA_Application = function () {
        this.web = new $DNA_Web();
        this.page = new $DNA_Page(this.web);
        this.widgets = {
            install: function (id, name, category) { return $DNA_FUC_InstallPKG("widget", id, name, category); },
            uninstall: function (name, category) {
                return $.ajax({
                    type: "POST",
                    url: $.resolveUrl("~/api/widgets/uninstall"),
                    data: { name: name, category: category },
                    error: function (jqXHR, textStatus, errorThrown) {
                        $.err(errorThrown);
                    }
                });
            },
            enable: function (name, category) {
                return $.ajax({
                    type: "POST",
                    url: $.resolveUrl("~/api/widgets/register"),
                    data: { name: name, category: category },
                    error: function (jqXHR, textStatus, errorThrown) {
                        $.err(errorThrown);
                    }
                });
            },
            disable: function (installPath) {
                return $.ajax({
                    type: "POST",
                    url: $.resolveUrl("~/api/widgets/unregister"),
                    data: { id: installPath },
                    error: function (jqXHR, textStatus, errorThrown) {
                        $.err(errorThrown);
                    }
                });
            },
            update: function (installPath) {
                return $.ajax({
                    type: "POST",
                    url: $.resolveUrl("~/api/widgets/update"),
                    data: { id: installPath },
                    error: function (jqXHR, textStatus, errorThrown) {
                        $.err(errorThrown);
                    }
                });
            }
        };
        this.themes = {
            install: function (id, name) { return $DNA_FUC_InstallPKG("theme", id, name); }
        };
        this.contentTypes = {
            install: function (id, name) { return $DNA_FUC_InstallPKG("list", id, name); },
            uninstall: function (name) {
                return $.ajax({
                    type: "POST",
                    url: $.resolveUrl("~/api/cloud/uninstall"),
                    data: { name: name, type: "contentType" },
                    error: function (jqXHR, textStatus, errorThrown) {
                        $.err(errorThrown);
                    }
                });
            }
        };
    };

    $.extend($DNA_Application, {
        login: function () { },
        logout: function () { }
    });

    $(function () {
        window.app = new $DNA_Application();
    });

})(jQuery, window, document);