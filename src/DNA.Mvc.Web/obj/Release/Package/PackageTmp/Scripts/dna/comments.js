(function ($) {
    $.widget("dna.commentList", $.dna.taoDataBindingList, {
        options: {
            scroller: "parent",
            insertMode: "append",
            reaonly: false
        },
        _create: function () {
            var self = this, opts = this.options, eventPrefix = this.widgetEventPrefix, el = this.element;
            el.addClass("d-comments");
            this._unobtrusive();
            if (el.data("readonly") != undefined)
                opts.reaonly = el.dataBool("readonly");

            if (opts.scroller)
                this._setScroller(opts.scroller);

            if (opts.datasource)
                this._setDataSource(opts.datasource);
        },
        _onItemCreated: function (element, data) {
            ///<remarks>About the comment object spec please visist: dev.dotnetage.com/social/comments </remarks>
            var self = this;
            element.addClass("d-item d-tran");
            var avatar = $("<a/>").appendTo(element)
                              .width(48)
                              .height(48)
                              .addClass("avatar")
                              .attr("href", data.actor.url)
                              .css({
                                  "background-image": "url(" + data.actor.image.url + "?w=64&h=64)"
                              });

            var contentEl = $("<div/>").appendTo(element).addClass("detail"),
            summryEl = $("<div/>").appendTo(contentEl).addClass("names");

            $("<a/>").attr("href", data.actor.url)
                           .addClass("dispName")
                           .text(data.actor.displayName)
                           .appendTo(summryEl);

            $("<a/>").attr("href", data.actor.url)
                           .addClass("userName")
                           .text("@" + data.actor.id)
                           .appendTo(summryEl);

            $("<span/>").addClass("posted")
                                    .text($.friendlyDate(data.published))
                                    .attr("title", $.jsonDate(data.published).toLocaleString())
                                    .attr("data-tooltip-width", 200)
                                    .appendTo(summryEl);

            $("<div/>").appendTo(contentEl).addClass("content")
                                .html(data.object.content);

            var tools = $("<div/>").appendTo(contentEl).addClass("tools"),
            replies_icon = $("<span/>").appendTo(tools).addClass("d-icon-bubbles-2");
            btnReplies = $("<span/>").text("replies (" + data.replies.totalItems + ")")
                                .appendTo(tools)
                                .addClass("replies")
                                .one("click", function () {
                                    var repliesHolder = $("<ul/>").appendTo(contentEl),
                                    editor = $("<div/>").appendTo(contentEl),
                                    _actions = self._source.taoDataSource("option", "actions");
                                    _actions.read.data.replyTo = data.id;
                                    if (_actions.insert && !self.options.reaonly)
                                        _actions.insert.data.replyTo = data.id;

                                    var _ds = $("<div/>").appendTo(contentEl);

                                    _ds.taoDataSource({
                                        actions: _actions,
                                        changed: function (event, results) {
                                            //btnReplies.text("replies (" + results.total + ")");
                                        }
                                    });

                                    repliesHolder.commentList({
                                        datasource: _ds,
                                        autoBind: true,
                                        readonly: self.options.reaonly
                                    });

                                    if (!self.options.reaonly) {
                                        editor.quickComment({
                                            datasource: _ds
                                        });
                                    }

                                    $(this).bind("click", function () {
                                        repliesHolder.slideToggle();
                                    });
                                });

            $("<span/>").appendTo(tools)
                                 .addClass("d-icon-flag")
                                 .click(function () {
                                     if ($.isFunction($.reportAbuse)) {
                                         $.reportAbuse(data.link, data.actor.id, data.object.objectType);
                                     }
                                 });

            contentEl.width(element.width() - 70);

            return this;
        }
    });

    //$.widget("dna.quickComment", {
    //    options: {
    //        datasource: null
    //    },
    //    _create: function () {
    //        var self = this, opts = this.options, eventPrefix = this.widgetEventPrefix, el = this.element;
    //        this._unobtrusive();

    //        if (!opts.datasource)
    //            throw "The quickComment's datasource cound not be null";

    //        this._setDataSource(opts.datasource);

    //        var cmmOpener = $("<div/>").addClass("textholder")
    //                                                      .appendTo(el)
    //                                                      .css({
    //                                                          "margin": "10px",
    //                                                          "padding": "5px",
    //                                                          "border": "1px solid #efefef",
    //                                                          "color": "#ccc"
    //                                                      })
    //                                                      .text("Add a comment ...")
    //                                                      .disableSelection(),

    //        cmmWrapper = $("<div/>").addClass("wrapper").css("padding", "10px")
    //                                                   .appendTo(el),
    //        textArea = $("<textarea/>").addClass("d-textarea")
    //                                                    .attr("maxlength", 255)
    //                                                    .appendTo(cmmWrapper)
    //                                                    .keypress(function (event) {
    //                                                        if (event.keyCode == 13 && $(this).val())
    //                                                            postButton.click();
    //                                                    })
    //                                                    .change(function () {
    //                                                        postButton.isDisable(textArea.val() == "");
    //                                                    })
    //                                                    .keyup(function () {
    //                                                        postButton.isDisable(textArea.val() == "");
    //                                                    }),
    //        buttonsHolder = $("<div/>").appendTo(cmmWrapper).css("margin-top", "5px"),
    //        postButton = $("<button/>").css({ width: "75px", "margin-right": "5px" })
    //                                                              .text("Post")
    //                                                              .appendTo(cmmWrapper)
    //                                                              .taoButton(),
    //        cancelButton = $("<button/>").css({ width: "75px" })
    //                                                                  .text("Cancel")
    //                                                                  .appendTo(cmmWrapper)
    //                                                                  .taoButton();
    //        textArea.width(cmmWrapper.width());
    //        postButton.click(function () {
    //            el.blockUI();
    //            var cmmObj = {
    //                content: textArea.val()
    //            };

    //            self._source.taoDataSource("insert", cmmObj).done(function () {
    //                textArea.val("");
    //                cancelButton.click();
    //                el.unblockUI();
    //            });
    //        })
    //        .isDisable(true);

    //        cmmWrapper.hide();

    //        cmmOpener.click(function () {
    //            cmmOpener.hide();
    //            cmmWrapper.show();
    //            textArea.width(cmmWrapper.width() - 8);
    //            textArea.focus();
    //        });

    //        cancelButton.click(function () {
    //            cmmOpener.show();
    //            cmmWrapper.hide();
    //        });
    //        this.commentEditorHolder = cmmOpener;
    //    },
    //    _unobtrusive: function (element) {
    //        var el = element ? element : this.element, opts = this.options;
    //        if (el.data("source"))
    //            opts.datasource = el.datajQuery("source");
    //    },
    //    open: function () {
    //        this.commentEditorHolder.click();
    //    },
    //    _setDataSource: function (val) {
    //        var self = this, opts = this.options;
    //        if (val) {
    //            //Binding to datasource widget
    //            if ($.isPlainObject(val)) {
    //                throw "Could not bind to none datasource object."
    //            } else {
    //                if (!val.jquery) throw "The input object is not a valid datasource object";
    //                var evtPrefix = "taoDataSource";

    //                if (this._source) {
    //                    this._source.unbind(evtPrefix + "position", $.proxy(this._onDataPosition, this))
    //                                     .unbind(evtPrefix + "inserted", $.proxy(this._onInserted, this))
    //                                     .unbind(evtPrefix + "updated", $.proxy(this._onUpdated, this))
    //                                     .unbind(evtPrefix + "removed", $.proxy(this._onRemoved, this))
    //                                     .bindunbind(evtPrefix + "process", $.proxy(this._onProcess, this))
    //                                     .unbind(evtPrefix + "error", $.proxy(this._onError, this))
    //                                     .unbind(evtPrefix + "completed", $.proxy(this._onCompleted, this));
    //                }

    //                this._source = val;

    //                this._source.bind(evtPrefix + "position", $.proxy(this._onDataPosition, this))
    //                                     .bind(evtPrefix + "inserted", $.proxy(this._onInserted, this))
    //                                     .bind(evtPrefix + "updated", $.proxy(this._onUpdated, this))
    //                                     .bind(evtPrefix + "removed", $.proxy(this._onRemoved, this))
    //                                     .bind(evtPrefix + "process", $.proxy(this._onProcess, this))
    //                                     .bind(evtPrefix + "error", $.proxy(this._onError, this))
    //                                     .bind(evtPrefix + "completed", $.proxy(this._onCompleted, this));
    //            }
    //        }
    //    },
    //    _onDataPosition: function (event, dataItem) {
    //        this._currentData = dataItem;
    //    },
    //    _onInserted: function (event, data) {
    //    },
    //    _onUpdated: function (event, data) {
    //    },
    //    _onRemoved: function (event, data) {
    //    },
    //    _onProcess: function (event, data) {
    //    },
    //    _onError: function (event, data) { },
    //    _onCompleted: function () {
    //        //this.element.unblockUI();
    //    },
    //    _triggerEvent: function (eventName, eventArgs) {
    //        this.element.trigger(this.widgetEventPrefix + eventName, eventArgs);
    //        return this;
    //    },
    //    disable: function () {
    //        this.widget().isDisable(true);
    //        return this;
    //    },
    //    enable: function () {
    //        this.widget().isDisable(false);
    //        return this;
    //    },
    //    //        _setOption: function (key, value) {
    //    //            return $.Widget.prototype._setOption.call(this, key, value);
    //    //        },
    //    destroy: function () {
    //        $.Widget.prototype.destroy.call(this);
    //    }
    //});


    $.widget("dna.commentbox", {
        options: {
            datasource: null,
            postText: "Post"
        },
        _create: function () {
            var el = this.element, opts = this.options, self = this;
            if (el.data("source"))
                opts.datasource = el.datajQuery("source");

            if (el.data("text-post"))
                opts.postText = el.data("text-post");

            el.wrap("<div/>");
            var wrapper = el.parent(),
            btns = $("<div/>").appendTo(wrapper).css("padding-top", "5px"),
            btnPost = $("<button/>").text(opts.postText)
                                                    .appendTo(btns)
                                                    .data("default", true).attr("data-inline",false)
                                                    .click(function () {
                                                        if (!$(this).isDisable()) {
                                                            if (!$("body").dataBool("auth")) {
                                                                $.login().done(function (data) {
                                                                    self._addComment();
                                                                });
                                                            } else
                                                                self._addComment();

                                                        } else
                                                            el.focus();
                                                    });

            btnPost.isDisable($.trim(el.val()) == "");
            el.addClass("d-textarea d-tran")
               .bind("focus", function () {
                   wrapper.isActive(true);
               })
               .bind("blur", function () {
                   wrapper.isActive(false);
               })
               .bind("keyup", function () {
                   btnPost.isDisable($.trim($(this).val()) == "");
               });
            wrapper.taoUI();
            wrapper.addClass("d-comment-box");

            //el.width(wrapper.innerWidth()-20);
            this.wrapper = wrapper;
        },
        _addComment: function () {
            var src = this.options.datasource, el = this.element, self = this;
            if (src) {
                self.disable();
                src.taoDataSource("insert", { content: el.val() }).done(function () {
                    el.val("");
                    self.wrapper.find("button").isDisable(true);
                    self.enable();
                });
            }
        },
        disable: function () {
            this.element.attr("disabled", "disabled").isDisable(true);
            this.wrapper.find("button").isDisable(true);
            return this.element;
        },
        enable: function () {
            this.element.removeAttr("disabled").isDisable(false);
            if (this.element.val() != "")
                this.wrapper.find("button").isDisable(false);
            return this.element;
        },
        destroy: function () {
            this.wrapper.after(this.element);
            //this.element.after(this.wrapper);
            this.wrapper.remove();
            this.element.removeClass("d-textarea").unbind();
            return $.Widget.prototype.destroy.call(this);
        }
    });

})(jQuery);