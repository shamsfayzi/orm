
createApp({
    data() {
        return {
            count: ref(0),
            //CountUnSeenComments: ref(0),
            notifications: [],
        }
    },
    methods: {
        async updateCount() {
            const thisVue = this
            await axios.get("/privateListManager/notifications/getCount")
                .then(function (response) {
                    thisVue.count = response.data
                })
                .catch(function (error) {
                    // handle error
                    console.log(error);
                });
        },
        async updateNotific() {
            const thisVue = this
            await axios.get("/privateListManager/notifications/GetNotifications")
                .then(function (response) {
                    thisVue.notifications = response.data
                })
                .catch(function (error) {
                    // handle error
                    console.log(error);
                });
        },
        async readCount() {
            const thisVue = this
            if (this.count > 0) {
                await axios.get("/privateListManager/notifications/readCount")
                    .then(function (response) {
                        thisVue.count = 0
                    })
                    .catch(function (error) {
                        // handle error
                        console.log(error);
                    });
            }
            this.updateNotific()
        },
        humanize(datetime) {
            return window.humanize(datetime)
        },
        toDateTime(datetime) {
            return window.toDateTime(datetime)
        }

    },
    created: function () {
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("/notificationHub")
            .configureLogging(signalR.LogLevel.Information)
            .build();
    },
    mounted: function () {
        var thisVue = this
        thisVue.connection.start()
        thisVue.connection.on("UpdateNotification", function () {
            thisVue.updateCount()
        });
        thisVue.connection.on("UpdateCommentSeenCount", function (obsId) {
            thisVue.UpdateCommentSeenCount(obsId)
        });
        thisVue.connection.on("notify", function (message) {
            alertify.success(message);
        });
        thisVue.updateCount();
        thisVue.updateNotific();
    }

}).mount('#appLayout')
