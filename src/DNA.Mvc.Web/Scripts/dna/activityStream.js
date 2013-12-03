/*
** Project : TaoUI
** Copyright (c) 2009-2013 DotNetAge (http://www.dotnetage.com)
** Licensed under the GPLv2: http://dotnetage.codeplex.com/license
** Project owner : Ray Liang (csharp2002@hotmail.com)
*/

(function ($) {
    $.widget("dna.activityStream", $.dna.taoDataBindingList, {
        options: {
            scroller: "window"
        },
        _create: function () {
            var self = this, opts = this.options, eventPrefix = this.widgetEventPrefix, el = this.element;
            el.addClass("d-stream");
            this._unobtrusive();
            opts.insertMode = "append";
            this._getItemsContainer();

            if (opts.scroller)
                this._setScroller(opts.scroller);

            if (opts.datasource)
                this._setDataSource(opts.datasource);

            this._removal = [];
        },
        _onItemCreated: function (element, data) {
            var self = this;
            if (data) {
                var _access = "", _privacy = -1;
                if (data.access) {
                    $.each(data.access.items, function (i, access) {
                        if (access.privacy > _privacy) {
                            _access = access.name;
                            _privacy = access.privacy;
                        }
                    });
                }

                var tb = $("<table/>").appendTo(element), row = $("<tr/>").appendTo(tb),
                avatar_cell = $("<td/>").css({ "width": "60px" }).appendTo(row),
                detailCell = $("<td/>").appendTo(row);
                avatar_cell.append($("<img/>").attr("src", data.actor.image.url).css("width", "48px").attr("data-tooltip-url", "/social/peoples/info/" + data.object.actor.id));
                detailCell.append($("<div/>").addClass("d-inline")
                                                                .append($("<a/>").attr("href", data.actor.url).text(data.actor.displayName).attr("data-tooltip-url", "/social/peoples/info/" + data.object.actor.id))
                                                                .append($("<span/>").addClass("d-published")
                                                                                                            .attr("title", $.jsonDate(data.published).toLocaleString())
                                                                                                            .text($.friendlyDate(data.published)))
                                                                .append($("<span/>").addClass("d-published").text(" - " + _access)));

                var lockedText = $("<span/>").text("(locked)")
                                                                .attr("title", "People you've shared this post will not be able to share it anyone else.")
                                                                .attr("data-tooltip-width", 200)
                                                                .addClass("d-published d-locked-text")
                                                                .appendTo(detailCell.children(".d-inline"));

                if (!data.locked)
                    lockedText.hide();

                var optionButton = $("<a/>").appendTo(detailCell)
                                                                       .css({
                                                                           "float": "right",
                                                                           "position": "absolute",
                                                                           "right": "0px",
                                                                           "top": "0px"
                                                                       })
                                                                       .attr("role", "link")
                                                                       .taoButton({
                                                                           primaryIcon: "ui-icon ui-icon-gear"
                                                                       }),
                                                                       _menuSrc = [{ text: "Report abuse"}];

                if ($.inArray("block", data.acl) > -1) {
                    _menuSrc.push({ text: "Block this person", click: function () {
                        var thisMenuItem = $(this);
                        $.post("/social/api/peoples/block", { userName: data.actor.id }).success(function () {
                            thisMenuItem.isDisable(true);
                        });
                    }
                    });
                }

                if ($.inArray("delete", data.acl) > -1) {
                    _menuSrc.push({ text: "Delete this post", click: function () {
                        $.confirm("Are you sure to delete this post ?").done(function () {
                            $.ajax({ url: data.object.url, type: "DELETE" }).done(function () {
                                element.remove();
                            });
                        });
                    }
                    });
                }

                if ($.inArray("lock", data.acl) > -1) {
                    var _textLock = "Lock this post", _textUnlcok = "Unlock this post",
                     _lockPostMenu = { text: data.locked ? _textUnlcok : _textLock, click: function () {
                         var _lockMenuElement = $(this);

                         $.ajax({ url: data.object.url, type: "PUT", data: { isLock: !data.locked} }).done(function () {
                             data.locked = !data.locked;
                             if (data.locked) {
                                 $(element).find(".d-share-button").hide();
                                 $(element).find(".d-locked-text").show();
                                 _lockMenuElement.find("span").text(_textUnlcok);
                             }
                             else {
                                 $(element).find(".d-share-button").show();
                                 $(element).find(".d-locked-text").hide();
                                 _lockMenuElement.find("span").text(_textLock);
                             }
                         });
                     }
                     };

                    if (data.locked)
                        _lockPostMenu.text = "Unlock this post";

                    _menuSrc.push(_lockPostMenu);
                }

                if ($.inArray("disable", data.acl) > -1) {
                    var _textDisableComments = "Disable comments", _textEnableComments = "Enable comments",
                     _disableMenu = { text: data.object.replies.disabled ? _textEnableComments : _textDisableComments, click: function () {
                         var _disableMenuElement = $(this);
                         $.ajax({ url: data.object.url, type: "PUT", data: { isDisable: !data.object.replies.disabled} }).done(function () {
                             data.object.replies.disabled = !data.object.replies.disabled;
                             if (data.object.replies.disabled) {
                                 $(element).find(".d-comments").hide();
                                 _disableMenuElement.find("span").text(_textEnableComments);
                             }
                             else {
                                 $(element).find(".d-comments").show();
                                 _disableMenuElement.find("span").text(_textDisableComments);
                             }
                         });
                     }
                     };
                    _menuSrc.push(_disableMenu);
                }

                var optionMenu = $("<ul/>").appendTo(detailCell)
                                                                .taoMenu({
                                                                    trigger: optionButton,
                                                                    type: "vertical",
                                                                    datasource: _menuSrc
                                                                });
                self._removal.push(optionMenu);
                if (data.object.annotation)
                    $("<p/>").appendTo(detailCell).text(data.object.annotation);

                var content = $("<div/>").addClass("d-content")
                                                        .appendTo(detailCell);

                if (data.id != data.object.id) {
                    content.addClass("share");

                    $("<div/>").appendTo(content)
                                      .addClass("d-inline")
                                      .append($("<img/>").css({ "width": "32px", "height": "32px" })
                                                                       .attr("src", data.object.actor.image.url))
                                      .append($("<a/>").attr("href", data.object.actor.url)
                                                                   .text(data.object.actor.displayName)
                                                   )
                }

                $("<p/>").html(data.object.body)
                               .appendTo(content);

                //Render the attachs
                this._renderAttachs(data, content, true);

                btnComments = $("<a/>").attr("role", "link")
                                                         .text("Comments (" + data.object.replies.totalItems + ")");

                $("<div/>").addClass("d-inline").css("clear", "left")
                                 .appendTo(detailCell)
                                  //.append($("<a/>").attr("role", "link")
                                  //                                       .text("Likes (" + data.object.likers.totalItems + ")")
                                  //                                       .click(function () {
                                  //                                           var linkLikes = $(this);
                                  //                                           $.post(data.object.likers.link, function (data) {
                                  //                                               linkLikes.taoButton("option", "label", "Likes (" + data + ")");
                                  //                                           });
                                  //                                       })
                                  //                                       .taoButton({ primaryImg: "/content/images/icon_heart_16.png" }))
                                      .append($("<a/>").attr("role", "link")
                                                                   .addClass("d-share-button")
                                                                   .text("Shares (" + data.object.reshares.totalItems + ")")
                                                                   .click(function () {
                                                                       self._sharePost(element, data);
                                                                   })
                                                                   .taoButton({ primaryImg: "/content/images/icon_refresh_16.png" })
                                                       )
                                      .append(btnComments.taoButton({ primaryImg: "/content/images/comment.png" }));

                if (data.locked)
                    element.find(".d-share-button").hide();

                var _popActors = function (popTarger, actors_url) {
                    $(popTarger).taoTooltip({
                        position: {
                            at: "left bottom",
                            my: "left top"
                        },
                        content: function () {
                            var _cEl = $("<div/>").css({ "max-height": "200px", "overflow": "auto" }).width(200);
                            $("<ul/>").appendTo(_cEl).peopleList({
                                datasource: {
                                    actions: {
                                        read: {
                                            url: actors_url,
                                            serverPaging: true
                                        }
                                    }
                                }
                            });
                            return _cEl;
                        }
                    })
                                            .taoTooltip("open");
                };

                //if (data.object.likers.totalItems) {
                //    $("<div/>").appendTo(detailCell)
                //                         .append($("<a/>").attr("role", "link")
                //                                                            .attr("href", "javascript:void(0);")
                //                                                            .text(data.object.likers.totalItems + " peoples like this")
                //                                                            .click(function (evt) {
                //                                                                evt.preventDefault();
                //                                                                _popActors(this, data.object.likers.link);
                //                                                            })
                //                                                            .taoButton());
                //}

                if (data.object.reshares.totalItems) {
                    $("<div/>").appendTo(detailCell)
                                         .append($("<a/>").attr("role", "link")
                                                                            .text(data.object.reshares.peoples + " peoples share this")
                                                                            .click(function (evt) {
                                                                                evt.preventDefault();
                                                                                _popActors(this, data.object.reshares.link);
                                                                            })
                                                                            .taoButton());
                }

                if (!data.object.replies.disabled) {

                    var _cbox = $("<div/>").appendTo(detailCell)
                                          .commentBox({
                                              totalItems: data.object.replies.totalItems,
                                              href: data.object.replies.link,
                                              latest: data.object.replies.latest
                                          });

                    btnComments.click(function () {
                        if (!data.object.replies.disabled)
                            _cbox.commentBox("open");
                    });
                }

                if (!this.delay) this.delay = 0;
                var delay = this.delay + 10;
                element.hide().delay(delay).slideDown()
                this.delay = delay;
            }
            return this;
        },
        _renderAttachs: function (data, container, browsable) {
            if (data.object.attachments) {
                if (data.object.attachments.length > 0) {
                    var tranformData = [];

                    $.each(data.object.attachments, function (i, t) {
                        tranformData.push({
                            url: t.url,
                            name: t.displayName,
                            owner: data.object.actor,
                            attrs: t.attrs
                        });
                    });

                    var thumbList = $("<div/>").css({ "overflow": "auto" }),
                    _viewAttach = function (_url) {
                        if (browsable) {
                            var viewer = $("<div/>").appendTo("body");
                            viewer.photoViewer({
                                viewUrl: _url,
                                bindingTo: {
                                    keyField: "url",
                                    data: tranformData
                                },
                                close: function () {
                                    viewer.remove();
                                }
                            });
                        }
                    };

                    for (var i = 0; i < 6; i++) {
                        if (i > (data.object.attachments.length - 1))
                            break;

                        var attach = data.object.attachments[i];

                        if (i == 0) {
                            if (attach.objectType == "article") {
                                container.append($("<a/>").attr("href", attach.url).attr("target","_blank").text(attach.displayName).append($("<span/>").text("- Reproduced").addClass("d-published")));
                                if (attach.image.url) {
                                    $("<img/>").addClass("d-shadow around").attr("src", attach.image.url);
                                }

                                container.append($("<p/>").html(attach.content));
                                return;
                            }
                            else {
                                container.append($("<img/>")
                                           .attr("src", attach.url + "?h=200&w=320")
                                           .data("url", attach.url)
                                           .addClass("d-shadow around")
                                           .css({ "margin-bottom": "5px", "cursor": "pointer" })
                                           .click(function () {
                                               _viewAttach($(this).data("url"));
                                           })
                                           );
                            }
                        }
                        else
                            $("<div/>").attr("src", attach.url + "?resize=60")
                                             .appendTo(thumbList)
                                              .data("url", attach.url)
                                             .css({
                                                 "background": "url(" + attach.url + "?resize=60) no-repeat center center",
                                                 "height": "60px",
                                                 "width": "60px",
                                                 "border": "1px solid #efefef",
                                                 "margin": "1px",
                                                 "float": "left",
                                                 "cursor": "pointer"
                                             }).click(function () {
                                                 _viewAttach($(this).data("url"));
                                             });
                    }
                    thumbList.appendTo(container);
                }
            }
        },
        _onInserted: function (val) {
            if (val) {
                this.options.insertMode = "prepend";
                this._addItem(val);
                this.options.insertMode = "append";
            }
        },
        _sharePost: function (element, data) {
            var self = this, opts = this.options;
            $.shareContentDlg("Share this post", function () {

                $("<div/>").appendTo(this)
                                      .addClass("d-inline")
                                      .append($("<img/>").css({ "width": "32px", "height": "32px" }).attr("src", data.object.actor.image.url))
                                      .append($("<a/>").attr("href", data.object.actor.url).text(data.object.actor.displayName));

                $("<p/>").html(data.object.content)
                              .appendTo(this);

                /*Render attachments*/
                self._renderAttachs(data, this);
            }).done(function (model) {

                model.activityId = data.object.id;

                $.post(data.object.reshares.link, model, function (response) {
                    opts.insertMode = "prepend";
                    self._addItem(response);
                    opts.insertMode = "append";
                });
            });
        },
        destroy: function () {
            var count = this._removal.length;
            for (var i = 0; i < count; i++) {
                var el = this._removal.pop();
                el.remove();
            }
            $.dna.taoDataBindingList.prototype.destroy.call(this);
        }
    });
})(jQuery);