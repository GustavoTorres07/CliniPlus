window.CliniPlusCalendar = {

    init: function (elementId, dotnetRef, initialDate) {

        let calendarEl = document.getElementById(elementId);

        let calendar = new FullCalendar.Calendar(calendarEl, {
            initialView: 'dayGridMonth',
            locale: 'es',
            selectable: true,
            headerToolbar: {
                left: 'prev,next today',
                center: 'title',
                right: ''
            },

            dateClick: function (info) {

                let today = new Date();
                today.setHours(0, 0, 0, 0);

                let clicked = new Date(info.date);
                clicked.setHours(0, 0, 0, 0);

                if (clicked < today) {
                    return;
                }

                dotnetRef.invokeMethodAsync('NotifyDateSelected', info.dateStr);
            }
        });

        calendar.render();

        return calendar;
    },

    highlightDate: function (calendar, dateStr) {
        if (calendar && typeof calendar.gotoDate === "function") {
            calendar.gotoDate(dateStr);
        }
    }
};
