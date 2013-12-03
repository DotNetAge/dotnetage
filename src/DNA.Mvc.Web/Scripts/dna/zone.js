/*
** Project : TaoUI
** Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
** Licensed under the GPLv2: http://dotnetage.codeplex.com/license
** Project owner : Ray Liang (csharp2002@hotmail.com)
*/

(function ($) {
    $.widget("dna.widgetZone", {
        options: {
            items: ".d-widget-zone",
            placeholder: "d-widget-placeholder",
            page: 0,
            sandbox: false
        },
        _create: function () {
            var self = this, opts = this.options, eventPrefix = this.widgetEventPrefix, el = this.element;
            el.sortable({
                items: ".d-widget,[data-role='widget-descriptor'],[data-role='widget-creator']",
                connectWith: opts.items,
                placeholder: opts.placeholder,
                forceHelperSize: false,
                forcePlaceholderSize: true,
                opacity: 0.9,
                start: function (event, ui) {
                    ui.item.hide();
                    //ui.helper.addClass("d-state-dragging");
                    //console.log(event);
                },
                tolerance: "pointer",
                // helper:"clone",
                helper: function (event, item) {
                    var txt = $.res("move", "Move") + " " + item.widget("option", "title") + " " + $.res("to", "to") + "<span class='text'>...</span>";
                    //console.log(event.clientX);
                    return $("<div/>").html(txt).addClass("d-widget-sortable-helper");
                },
                cursor: "move",
                //handle: ".d-widget-header,.d-widget-drag-handler",
                cancel: ".d-widget-viewmode-floating,[contenteditable=true],[data-editable=true]",
                stop: function (event, ui) {
                    //  ui.item.removeClass("d-state-dragging");
                    ui.item.show();

                    var srcEle = ui.item, pos = srcEle.index();

                    if (srcEle.data("role") == "widget-descriptor") {
                        if (opts.sandbox) {
                            srcEle.empty();
                            srcEle.widget();
                        } else {
                            var addLoader = $("<div/>").css("padding", "10px");
                            $("<span/>").addClass("d-icon-loading d-inline").appendTo(addLoader);
                            $("<span/>").addClass("d-inline").appendTo(addLoader).text($.res("AddingWidget", "Adding widget ..."));

                            srcEle.after(addLoader);
                            srcEle.remove();

                            $.ajax({
                                url: $.resolveUrl("~/api/" + $("body").data("web") + "/widgets/add?locale=" + $("body").attr("lang")),
                                type: "POST",
                                data: {
                                    id: srcEle.data("id"),
                                    pageId: $("body").data("id"),
                                    zoneID: el.attr("id"),
                                    pos: pos,
                                    url: document.URL
                                },
                                error: function (jqXHR, textStatus, errorThrown) {
                                    addLoader.remove();
                                    $.err("There is an error occur! Can not add this widget to page. The error detail is :" + errorThrown);
                                }
                            })
                              .done(function (data) {
                                  if (data.error) {
                                      $.err(data.error);
                                      return;
                                  }

                                  var w = $("<div id=\"widget_" + data.id + "\"/>").attr("data-role", "widget").attr("data-zone", el.attr("id"));
                                  addLoader.after(w);
                                  addLoader.remove();
                                  w.widget(data);
                              });
                        }
                    }

                    if (srcEle.data("role") == "widget-creator") {

                        var addLoader = $("<div/>").css("padding", "10px");
                        $("<span/>").addClass("d-loading d-inline").appendTo(addLoader).width(16).height(16);
                        $("<span/>").addClass("d-inline").appendTo(addLoader).text($.res("AddingWidget", "Adding widget ..."));

                        srcEle.after(addLoader);
                        srcEle.remove();

                        var _dat = {
                            pageId: $("body").data("id"),
                            zoneID: el.attr("id"),
                            pos: pos,
                            url: document.URL,
                            locale: $('body').attr("lang")
                        }, attrs = srcEle[0].attributes;

                        for (var i = 0; i < attrs.length; i++) {
                            var a = attrs[i];
                            if (a.localName != "data-role" && a.localName != "data-url") {
                                _dat[a.localName.replace("data-", "")] = a.value;
                            }
                        }

                        $.ajax({
                            url: srcEle.data("url"),
                            type: "post",
                            data: _dat,
                            error: function (jqXHR, textStatus, errorThrown) {
                                $.err("There is an error occur! Can not add this widget to page. The error detail is :" + errorThrown);
                            }
                        }).done(function (data) {

                            if (data.error) {
                                $.err(data.error);
                                return;
                            }

                            if ($.isArray(data)) {
                                var currentEle = addLoader;

                                $.each(data, function (i, wdat) {
                                    var w = $("<div id=\"widget_" + data.id + "\"/>").attr("data-role", "widget");
                                    currentEle.after(w);
                                    w.widget(wdat);
                                    currentEle = w;
                                });

                                addLoader.remove();
                            } else {
                                var w = $("<div id=\"widget_" + data.id + "\"/>").attr("data-role", "widget");
                                addLoader.after(w);
                                addLoader.remove();
                                w.widget(data);
                            }
                        });

                    }
                },
                update: function (event, ui) {
                    if (ui.item.data("role") == "widget") {
                        try {
                            var widgets = $(".d-widget", this), _pos = -1;
                            if (widgets.length > 0)
                                _pos = widgets.index(ui.item);

                            if (_pos != -1)
                                ui.item.widget("move", $(this).attr("id"), _pos);
                            // ui.item.portlet("move", $(this).attr("id"), _pos);
                        }
                        catch (e) {
                            $(ui.sender).sortable('cancel');
                        }
                    }
                }
            }).addClass("d-widget-zone-design")
               .droppable({
                   accept: "[data-role='widget']:not('.d-widget-viewmode-floating'),[data-role='widget-descriptor'],[data-role='widget-creator']",
                   over: function (event, ui) {
                       ui.helper.find("span.text").text($(this).attr("data-label") ? $(this).attr("data-label") : $(this).attr("id"));
                   }
                   //activeClass: "d-widget-zone-allow-drop"
                   //hoverClass: "d-widget-zone-dag-hover",
                   //out: function () {
                   //el.removeClass("d-widget-zone-dag-hover");
                   //}
               })
               .fileDroppable({
                   stop: function (e, files) {
                       var allImages = true, _imgs = [];
                       if (files.length > 1) {
                           for (var i = 0; i < files.length; i++) {
                               var _f = files[i];
                               if (_f.type == "" || !_f.type.startsWith("image")) {
                                   allImages = false;
                                   break;
                               }
                           }
                       } else
                           allImages = false;

                       for (var i = 0; i < files.length; i++) {
                           var file = files[i],
                               _path = file.type ? (file.type.split("/")[0] + "s/") : "",
                               _url = $.resolveUrl("~/webshared/" + $("body").attr("data-web") + "/" + _path);

                           if (allImages) {
                               _imgs.push({
                                   title: "",
                                   image: _url + file.name,
                                   link: _url + file.name
                               });
                           }

                           $.upload(_url, file, null, function (event, xhr, status, response) {
                               if (!allImages) {
                                   var fileJson = $.parseJSON(response), addLoader = $("<div/>").css("padding", "10px");
                                   $("<span/>").addClass("d-loading d-inline").appendTo(addLoader).width(16).height(16);
                                   $("<span/>").addClass("d-inline").appendTo(addLoader).text($.res("AddingWidget", "Adding widget ..."));
                                   el.prepend(addLoader);
                                   $.ajax({
                                       url: $.resolveUrl("~/api/" + $("body").attr("data-web") + "/widgets/addfile?locale=" + $("body").attr("lang")),
                                       type: "post",
                                       data: {
                                           pageId: $("body").data("id"),
                                           zoneID: el.attr("id"),
                                           pos: 0,
                                           url: document.URL,
                                           locale: $('body').attr("lang"),
                                           file: fileJson.url
                                       },
                                       error: function (jqXHR, textStatus, errorThrown) {
                                           $.err("There is an error occur! Can not add this widget to page. The error detail is :" + errorThrown);
                                       }
                                   }).done(function (data) {
                                       if (data.error) {
                                           $.err(data.error);
                                           return;
                                       }

                                       if ($.isArray(data)) {
                                           var currentEle = addLoader;

                                           $.each(data, function (i, wdat) {
                                               var w = $("<div id=\"widget_" + data.id + "\"/>").attr("data-role", "widget");
                                               currentEle.after(w);
                                               w.widget(wdat);
                                               currentEle = w;
                                           });

                                           addLoader.remove();
                                       } else {
                                           var w = $("<div id=\"widget_" + data.id + "\"/>").attr("data-role", "widget");
                                           addLoader.after(w);
                                           addLoader.remove();
                                           w.widget(data);
                                       }
                                   });
                               }
                           });

                       }

                       if (allImages) {
                           var addLoader = $("<div/>").css("padding", "10px");
                           $("<span/>").addClass("d-loading d-inline").appendTo(addLoader).width(16).height(16);
                           $("<span/>").addClass("d-inline").appendTo(addLoader).text($.res("AddingWidget", "Adding widget ..."));
                           el.prepend(addLoader);

                           $.ajax({
                               url: $.resolveUrl("~/api/" + $("body").attr("data-web") + "/widgets/addSlideShow?locale=" + $("body").attr("lang")),
                               type: "post",
                               data: {
                                   pageId: $("body").data("id"),
                                   zoneID: el.attr("id"),
                                   pos: 0,
                                   url: document.URL,
                                   locale: $('body').attr("lang"),
                                   json: JSON.stringify(_imgs)
                               },
                               error: function (jqXHR, textStatus, errorThrown) {
                                   $.err("There is an error occur! Can not add this widget to page. The error detail is :" + errorThrown);
                               }
                           }).done(function (data) {
                               if (data.error) {
                                   $.err(data.error);
                                   return;
                               }

                               if ($.isArray(data)) {
                                   var currentEle = addLoader;

                                   $.each(data, function (i, wdat) {
                                       var w = $("<div id=\"widget_" + data.id + "\"/>").attr("data-role", "widget");
                                       currentEle.after(w);
                                       w.widget(wdat);
                                       currentEle = w;
                                   });

                                   addLoader.remove();
                               } else {
                                   var w = $("<div id=\"widget_" + data.id + "\"/>").attr("data-role", "widget");
                                   addLoader.after(w);
                                   addLoader.remove();
                                   w.widget(data);
                               }
                           });
                       }
                   }
               });
        },
        settings: function () { },
        changestyle: function () { },
        bingfront: function () {
            this.element.css("z-index", $.topMostIndex());
            this.save();
        },
        sendback: function () {
            this.element.css("z-index", 2);
            this.save();
        },
        save: function () {
            $("body").trigger("layoutchange", { layout: true });
        },
        destroy: function () {
            this.element.sortable("destroy").droppable("destroy");
            this.element.unbind();
            this.element.removeClass("d-widget-zone-design");
            $.Widget.prototype.destroy.call(this);
        }
    });

    $.widget("dna.widgetDescriptor", {
        options: {
            connectTo: null
        },
        _create: function () {
            var self = this, opts = this.options, eventPrefix = this.widgetEventPrefix, el = this.element;
            el.draggable({
                connectToSortable: ".d-widget-zone",
                appendTo: "body",
                helper: function () {
                    return $("<div/>").attr("data-role", "widget-descriptor")
                                                .addClass("d-widget-desc-helper")
                                                .css("z-Index", $.topMostIndex)
                                                .html($.res("add", "Add") + " <strong>" + el.text() + "</strong> " + $.res("to", "to") + " ...");
                }
            });
        },
        destroy: function () {
            $.Widget.prototype.destroy.call(this);
        }
    });
})(jQuery);