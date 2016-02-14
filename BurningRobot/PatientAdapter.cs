using Android.App;
using Android.Text;
using Android.Views;
using Android.Widget;
using Hl7.Fhir.Model;

namespace BurningRobot
{
    public class PatientAdapter : ArrayAdapter<Bundle.EntryComponent>
    {
        private Activity _activity;

        public PatientAdapter(Activity activity) : base(activity, Resource.Layout.PatientViewItem)
        {
            _activity = activity;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView ?? _activity.LayoutInflater.Inflate(
                Resource.Layout.PatientViewItem, parent, false);
            var patientNameView = view.FindViewById<TextView>(Resource.Id.PatientName);
            Patient patient = GetItem(position).Resource as Patient;
            patientNameView.TextFormatted = Html.FromHtml(patient.Text.Div);

            return view;
        }
    }
}
