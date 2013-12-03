(function ($) {
    $.widget("dna.poll", {
        options: {
            data: null,
            postData: null,
            multi: false,
            readonly:false,
            user: "",
            url: null,
            votes: true,
            anonymous:true
        },
        _create: function () {
            var self = this, opts = this.options, eventPrefix = this.widgetEventPrefix, el = this.element;
            this._unobtrusive();
            el.addClass("d-poll");
            this._initUI(opts.data);
            return el;
        },
        getTotalVotes: function () { return this.totalVotes; },
        getPeoples: function () {
            return this.getVoters().length;
        },
        getVoters: function () {
            var opts = this.options, total = 0, peoples = [];;
            if (opts.data) {
                for (var i = 0; i < opts.data.length; i++) {
                    var opt = opts.data[i];
                    if (opt.users) {
                        var users = (opt.users && opt.users != "") ? opt.users.split(",") : [];
                        $.each(users, function (j, usr) {
                            if ($.inArray(usr, peoples) == -1)
                                peoples.push(usr);
                        });
                    }
                }
            }
            return peoples;
        },
        submit: function () {
            var self = this, opts = this.options, el = this.element,
                postData = opts.postData,
                results = this._getResult();

            if (opts.url) {
                if (postData) {
                    postData.peoples = this.totalVotes + 1;
                    postData.options = JSON.stringify(results);
                }
                else
                    postData = {
                        peoples: this.totalVotes + 1,
                        options: JSON.stringify(results)
                    };

                //opts.data = results;
                //el.empty();
                //self._initUI(results);
                //el.taoUI();

                $(".d-button", el).isDisable(true);

                return $.ajax({
                    url: opts.url,
                    data: postData,
                    type: "post",
                    error: function () {
                        $.err("There are error occur during post poll data.");
                        $(".d-button", el).isDisable(false);
                    }, success: function () {
                        opts.data = results;
                        el.empty();
                        self._initUI(results);
                       // el.taoUI();
                    }
                });
            }

            return el;
        },
        _unobtrusive: function (element) {
            var el = element ? element : this.element, opts = this.options;
            if (el.data("options"))
                opts.data = el.data("options");
            if (el.data("post-data"))
                opts.postData = el.data("post-data");
            if (el.data("votes") != undefined)
                opts.votes = el.dataBool("votes");
            if (el.data("readonly") != undefined)
                opts.readonly = el.dataBool("readonly");
            if (el.data("url"))
                opts.url = el.data("url");
            if (el.data("user"))
                opts.user = el.data("user");
            if (el.data("multi") != undefined)
                opts.multi = el.dataBool("multi");
            if (el.data("anonymous") != undefined)
                opts.anonymous = el.dataBool("anonymous");
            
        },
        _triggerEvent: function (eventName, eventArgs) {
            this.element.trigger(this.widgetEventPrefix + eventName, eventArgs);
            return this;
        },
        disable: function () {
            this.widget().isDisable(true);
            return this;
        },
        enable: function () {
            this.widget().isDisable(false);
            return this;
        },
        _isVoted: function () {
            var opts = this.options;
            if (opts.data) {
                for (var i = 0; i < opts.data.length; i++) {
                    var opt = opts.data[i];
                    if (opt.users) {
                        var users = (opt.users && opt.users != "") ? opt.users.split(",") : [];
                        if ($.inArray(opts.user, users) > -1)
                            return true;
                    }
                }
            }
            return false;
        },
        _getResult: function () {
            var self = this, opts = this.options, el = this.element, results = [];
            $(".d-poll-option", el).each(function (i, opt) {
                var optEl = $(opt),
                    oData = optEl.data("data"),
                    ckb = $(":checked", optEl);

                if (ckb.length) {
                    var users = oData.users;

                    if (users == "" || users == [] || users == undefined || users == null)
                        users = [];
                    else
                        users = users.split(",");

                    if ($.inArray(opts.user, users) == -1) {
                        users.push(opts.user);
                        oData.users = users.join(",");
                        oData.value++;
                    }
                }
                results.push(oData);
            });
            return results;
        },
        _initUI: function (data) {
            var self = this, opts = this.options, el = this.element;
            if (data) {
                var total = this.getPeoples();
                this.totalVotes = total, isVoted = self._isVoted(), _group = "";

                if (!opts.multi) {
                    var prefix = "poll_options_"
                    k = 0;
                    _group = prefix + k;
                    while ($("#" + _group).length) {
                        k++;
                        _group = _group = prefix + k;
                    }
                }

                $.each(data, function (i, val) {
                    var optEl = $("<div/>").appendTo(el).addClass("d-poll-option").attr("data-val", val.value).data("data", val),
                        optLb = $("<div/>").appendTo(optEl).addClass("d-poll-option-label"),
                        optChk = $("<input type='" + (opts.multi ? "checkbox" : "radio") + "' data-role='none' />").attr("name", opts.multi ? val.name : _group)
                                                                                           .attr("data-label", val.title)
                                                                                           .appendTo(optLb),
                        optprogressHolder = $("<div/>").appendTo(optEl).addClass("d-poll-option-progress"),
                            optProgress = $("<div/>").appendTo(optprogressHolder);

                   if (val.users && val.users != "") {
                        var _users = val.users.split(",");
                        if ($.inArray(opts.user, _users) > -1)
                            optChk.attr("checked", "checked");
                    }

                   if (!opts.multi) {
                       optChk.val(val.name);
                       optChk.taoRadio();
                   } else {
                       optChk.taoCheckbox();
                   }

                    if (isVoted || opts.readonly) {
                        optProgress.taoProgressbar({
                            value: val.value,
                            max: total
                        });

                        $("<span/>").addClass("d-inline")
                                             .text(optProgress.taoProgressbar("getPercentage") + "%")
                                             .prependTo(optprogressHolder);
                    }

                });

                var btnHolder = $("<div/>").addClass("d-poll-button-holder").appendTo(el),
                    btnVote = $("<a data-default='true'/>").text("Vote").addClass("d-state-disable")
                                                                                                                  .width(100)
                                                                                                                  .appendTo(btnHolder).click(function () {
                                                                                                                      if (opts.anonymous || (!opts.anonymous && $.isAuth()))
                                                                                                                          self.submit();
                                                                                                                      else {
                                                                                                                          $.login()
                                                                                                                            .done(function () {
                                                                                                                                self.submit();
                                                                                                                            });
                                                                                                                      }
                                                                                                                  });
                btnVote.taoButton();

                $("<label/>").text(total).appendTo(btnHolder);

                if (!isVoted && !opts.readonly) {
                    $("input", el).on("change", function () {
                        btnVote.isDisable($(":checked", el).length == 0);
                    });
                } else {
                    $(".d-radio,.d-checkbox", el).addClass("d-state-disable");
                    btnVote.remove();
                }
            }
            return el;
        },
        //        _setOption: function (key, value) {
        //            return $.Widget.prototype._setOption.call(this, key, value);
        //        },
        destroy: function () {
            $.Widget.prototype.destroy.call(this);
        }
    });
})(jQuery);

$(function () {
    $("[data-role=poll]").poll();
});