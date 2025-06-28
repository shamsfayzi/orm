const { reactive, createApp, ref } = Vue
const store = reactive({
    approvalCount: 0,
    async updateApprovalCount() {
        let thisVue = this
        await axios.get("/GetStockApprovalCount").then(function (res) {
            thisVue.approvalCount = res.data
        }).catch(function (error) {
            console.log(error)
        })
    }
})
createApp({
    data() {
        return {
            count: ref(0),
            notifications: [],
            store
        }
    },
    methods: {
        async updateCount() {
            const thisVue = this
            await axios.get("/notifications/getCount")
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
            await axios.get("/notifications/GetNotifications")
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
                await axios.get("/notifications/readCount")
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
        thisVue.connection.on("notify", function (message) {
            alertify.success(message);
        });
        thisVue.updateCount()
        thisVue.updateNotific()
        thisVue.store.updatePurchaseCount(true)
        thisVue.store.updateReservationCount(true)
    }

}).mount('#appLayout')
