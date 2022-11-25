using Microsoft.AspNetCore.Mvc.Rendering;

namespace MedBrat.Areas.Appointment.ViewModels
{
    public class TicketsFilterViewModel
    {
        public List<MedTicketViewModel> Tickets { get; }
        public string SelectedName { get; }
        public int SelectedTimePeriod { get; }
        public SelectList TimePeriods { get; }

        public TicketsFilterViewModel(List<MedTicketViewModel> tickets, int timePeriod, string name)
        {
            Tickets = tickets;
            var timePeriods = new List<TimePeriod>
            {
                new TimePeriod() { Id=55,Name="Все"},
                new TimePeriod() { Id=0,Name="Сегодня"},
                new TimePeriod() { Id=1,Name="Неделя"},
                new TimePeriod() { Id=4,Name="Месяц"},
                new TimePeriod() { Id=-1,Name="Просрочено"}
            };
            TimePeriods = new SelectList(timePeriods, "Id", "Name", timePeriod);
            SelectedTimePeriod = timePeriod;
            SelectedName = name;
        }

        private class TimePeriod
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
