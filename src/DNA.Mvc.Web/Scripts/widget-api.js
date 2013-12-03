WindowWidget = function (pros, prefs) {
    for (var p in pros)
        this[p] = pros[p];

    if (prefs == undefined)
        this.preferences = [];
    else
        this.preferences = prefs;

    this.preferences.getItem = function (name) {
        for (var i = 0; i < this.length; i++) {
            if (this[i].name == name)
                return this[i].value;
        }
        return null;
    };
    this.parent = window;
    this._eventlist = [];
    this.preferences.setItem = function (name, value) {
        for (var i = 0; i < this.length; i++) {
            if (this[i].name == name) {
                if (this[i].readonly) {
                    throw "The specified perference is readonly.";
                }
                else {
                    this[i].value;
                    __savePrefValueToStroage(name, value);
                }
                return;
            }
        }

        this.push({
            name: name,
            value: value,
            readonly: false
        });
        __savePrefValueToStroage(name, value);
    };
}

WindowWidget.prototype = {
    setIcon: function (src) {
        var img = new Image(), link = $(".d-widget-title-link", this.getHeaderElement());
        $(img).bind("load", function () {
            if (link.children("img").length > 0)
                link.children("img").attr("src", src).height(16).width(16);
            else
                $(this).prependTo(link).height(16).width(16);
        })
                  .attr("src", src)
    },
    setLink: function (href) {
        var link = $(".d-widget-title-link", this.getHeaderElement());
        if (link.length)
            link.attr("href", href);
    },
    setTitle: function (val) {
        var header = $(".d-widget-title-text", this.getHeaderElement());
        header.text(val);
    },
    saveSettings: function (options) {

    },
    getElement: function () {
        return window.parent.document.getElementById(widget.identifier);
    },
    link: function (val) {
        this.setLink(val);
        return this;
    },
    text: function (val) {
        this.setTitle(val);
        return this;
    },
    height: function (val) {
        if (val != undefined) {
            $(this.getBodyElement()).height(val);
            this._height = val;
            return this;
        } else
            return this._height;
    },
    width: function (val) {
        if (val != undefined) {
            $(this.getBodyElement()).width(val);
            this._width = val;
            return this;
        } else
            return this._width;
    },
    getBodyElement: function () {
        return $(".d-widget-frame", this.getElement())[0];
    },
    getHeaderElement: function () {
        return $(".d-widget-header", this.getElement())[0];
    },
    openApplication: function (bundleId) {
    },
    openURL: function (url) {
        window.open(url);
    },
    setCloseBoxOffset: function (x, y) { },
    system: function (command, endHandler) { },
    preferenceForKey: function (key) {
        return this.preferences.getItem(key);
    },
    prepareForTransition: function (transition) {
    },
    performTransition: function () {
    },
    setPreferenceForKey: function (preference, key) {
        return this.preferences.setItem(preference, key);
    },
    bind: function (evt, callback) {
        this._eventlist.push({ name: evt, callback: $.proxy(callback, this) });
    },
    undbind: function (evt) {
        var nlist = [];
        for (var i = 0; i < this._eventlist.length; i++) {
            if (this._eventlist[i].name != evt)
                nlist.push(this._eventlist[i]);
        }
        this._eventlist = nlist;
    },
    _triggerEvent: function (evt) {
        for (var i = 0; i < this._eventlist.length; i++) {
            if (this._eventlist[i].name == evt)
                this._eventlist[i].callback();
        }
    },
    _init: function () {
        var self = this, _posturi = __appPath + "api/widgets/apply";
        var userprefs = $("[data-role='userprefs'],.d-widget-prefs");

        //if (userprefs.length) {
        //    userprefs.attr("data-ajax", true)
        //        .attr("data-ajax-method", "post")
        //        .attr("data-ajax-url", _posturi);
        //}

        this.bind("design", function () {
            var userprefs = $("[data-role='userprefs'],.d-widget-prefs");
            if (userprefs.length) {
                var inputs = $(":input,:text", userprefs);
                inputs.each(function (i, n) {
                    var _name = $(n).attr("name");
                    if (_name) {
                        var _val = self.preferenceForKey(_name);
                        if (_val != undefined) {
                            if ($(n).attr("type") == "checkbox") {
                                if (_val == true || _val == "true" || _val == "True")
                                    $(n).attr("checked", "checked").val("True");
                            }
                            else
                                $(n).val(_val);
                        }
                    }
                });

                //var allowPop = userprefs.data("allow-pop") != undefined ? userprefs.dataBool("allow-pop") : true;
                //if (!allowPop) {
                //    var container = $("<p/>").appendTo(userprefs);
                //    $("<a/>").text("Save")
                //                               .click(function () {
                //                                   $.ajax({
                //                                       url: _posturi,
                //                                       type: "POST",
                //                                       data: userprefs.serialize()
                //                                   }).done(function () {
                //                                       editbar.show();
                //                                       userprefs.hide();
                //                                   });
                //                               })
                //                               .appendTo(container).taoButton();

                //    //If not allow popu show the setting button
                //    var editbar = $("<div/>").prependTo("body")
                //                    .css({
                //                        "text-align": "center",
                //                        "padding": "5px 0px",
                //                        "background-color": "#f2f2f2"
                //                    })
                //                    .append($("<a/>").width(100).text("Edit").click(function () {
                //                        userprefs.show();
                //                        editbar.hide();
                //                    }).taoButton());
                //}
            }
        });

        this._triggerEvent("load");
        //console.log("On widget load");
    }
}

function __savePrefValueToStroage(name, value) {
    var postData = { id: window.widget.identifier };
    postData[name] = value;
    try {
        $.ajax({
            type: "POST",
            url: __appPath + "api/widgets/apply",
            data: postData
        });
    } catch (e) { console.log(e); }
}