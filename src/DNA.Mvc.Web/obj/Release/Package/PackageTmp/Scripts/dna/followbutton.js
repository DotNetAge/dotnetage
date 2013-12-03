(function ($) {
    $.widget("dna.followButton", $.dna.taoButton, {
        options: {
            objectID: 0,
            userName: "",
            followUrl: "/api/contents/follow",
            unfollowUrl: "/api/contents/unfollow",
            followingUrl: "/api/contents/isfollowing",
            autocheck: false,
            isfollowing: false,
            followText: "Follow",
            unfollowText: "Unfollow"
        },
        _create: function () {
            var self = this, opts = this.options, eventPrefix = this.widgetEventPrefix, el = this.element;
            $.dna.taoButton.prototype._create.call(this);
            self._toggleFollowState();

            if (opts.objectID == 0 && opts.userName == "") {
                $.err("Please set the album or userName of this widget.");
                this.disable();
                return;
            }

            if (opts.autocheck) {
                this.disable();
                $.ajax({
                    url: opts.followingUrl + "/" + opts.objectID,
                    data: { owner: opts.userName }
                }).done(function (data) {
                    opts.isfollowing = data;
                    self._toggleFollowState();
                }).always(function () {
                    self.enable();
                });
            }

            el.click(function (e) {
                e.stopPropagation();
                e.preventDefault();
                if (el.isDisable())
                    return;

                self.disable();
                $.post((opts.isfollowing ? opts.unfollowUrl : opts.followUrl) + "/" + opts.objectID, { owner: opts.userName }, function (data) {
                    opts.isfollowing = data;
                    self._toggleFollowState(true);
                    self.enable();
                });
            });
        },
        _toggleFollowState: function (showMsg) {
            var self = this, opts = this.options, eventPrefix = this.widgetEventPrefix, el = this.element;
            if (opts.isfollowing) {
                this._setOption("label", opts.unfollowText);
                if (showMsg) {
                    if (opts.objectID)
                        $.notify("You are following this album now.");
                    else
                        $.notify("You are following all albums of " + opts.userName + " now.");
                }
            }
            else {
                this._setOption("label", opts.followText);
                if (showMsg) {
                    if (opts.objectID)
                        $.notify("This album has been removed from your following list.");
                    else
                        $.notify("You are unfollow all albums of this user.");
                }
            }
        },
        _unobtrusive: function () {
            var self = this, opts = this.options, el = this.element;
            if (el.data("id") != undefined) opts.objectID = el.dataInt("id");
            if (el.data("auto-check") != undefined) opts.autocheck = el.dataBool("auto-check");
            if (el.data("isfollowing") != undefined) opts.isfollowing = el.dataBool("isfollowing");
            if (el.data("user")) opts.userName = el.data("user");
            if (el.data("follow-url")) opts.followUrl = el.data("follow-url");
            if (el.data("unfollow-url")) opts.followUrl = el.data("unfollow-url");
            if (el.data("following-url")) opts.checkUrl = el.data("following-url");
            if (el.data("follow-text")) opts.followText = el.data("follow-text");
            if (el.data("unfollow-text")) opts.unfollowText = el.data("unfollow-text");
            return $.dna.taoButton.prototype._unobtrusive.call(this);
        }
    });
})(jQuery);