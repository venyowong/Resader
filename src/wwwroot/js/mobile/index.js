var app = new Vue({
    el: '#app',
    data: function () {
        return {
            active: "home-container",
            selected: "home",
            tabbarFixed: true
        };
    },
    methods: {

    },
    watch: {
        selected(val, oldVal) {
            app.active = `${val}-container`;
        }
    }
});