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
                // Avisar a Blazor qué fecha se seleccionó
                // ESTE NOMBRE DE MÉTODO DEBE COINCIDIR CON [JSInvokable] EN EL COMPONENTE RAZOR
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
