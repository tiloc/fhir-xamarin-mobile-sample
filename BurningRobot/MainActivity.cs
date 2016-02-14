using Android.App;
using Android.Util;
using Android.Widget;
using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using System;
using System.Threading;



namespace BurningRobot
{
    [Activity(Label = "BurningRobot", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        static readonly string TAG = "BurningRobot:" + typeof(MainActivity).Name;

        protected override void OnCreate(Android.OS.Bundle bundle)
        {
            base.OnCreate(bundle);

            if (bundle == null)
            {
                Log.Info(TAG, "OnCreate bundle is null");
            }

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button fetchPxButton = FindViewById<Button>(Resource.Id.FetchPatientsButton);

            if (fetchPxButton == null)
            {
                Log.Error(TAG, "OnCreate button is null");
            }

            ListView pxListView = FindViewById<ListView>(Resource.Id.PatientListView);
            pxListView.Adapter = new PatientAdapter(this);

            fetchPxButton.Click += delegate {
                DisableInputs();
                ThreadPool.QueueUserWorkItem(o => FetchPatients());
            };
        }

        private void FetchPatients()
        {
            FhirClient fhirClient = new FhirClient("https://fhir-open.sandboxcernerpowerchart.com/dstu2/d075cf8b-3261-481d-97e5-ba6c48d3b41f");
            fhirClient.PreferredFormat = ResourceFormat.Json;
            fhirClient.UseFormatParam = true;

            try
            {
                EditText patientNameField = FindViewById<EditText>(Resource.Id.PatientNameField);
                string patientName = patientNameField.Text;
                SearchParams searchParams = new SearchParams();
                searchParams.Add("name", patientName);
                searchParams.Add("_count", "50");
                Hl7.Fhir.Model.Bundle patients = fhirClient.Search<Patient>(searchParams);
                Log.Info(TAG, "Retrieved patients: " + patients.Total);

                RunOnUiThread(() =>
                {
                    ListView pxListView = FindViewById<ListView>(Resource.Id.PatientListView);
                    ArrayAdapter adapter = pxListView.Adapter as ArrayAdapter;
                    Log.Debug(TAG, "Adapter: " + adapter.ToString());
                    adapter.Clear();
                    adapter.AddAll(patients.Entry);
                }
                );
            }
            catch (Exception e)
            {
                Log.Warn(TAG, e.Message);
                RunOnUiThread(() =>
                {
                    Android.Widget.Toast.MakeText(this, e.Message, ToastLength.Long).Show();
                }
                );
            }
            finally
            {
                RunOnUiThread(() => {
                    EnableInputs();
                }
                );
            }
        }


        private void EnableInputs()
        {
            Button fetchPxButton = FindViewById<Button>(Resource.Id.FetchPatientsButton);
            fetchPxButton.Enabled = true;
            EditText patientNameField = FindViewById<EditText>(Resource.Id.PatientNameField);
            patientNameField.Enabled = true;
        }

        private void DisableInputs()
        {
            Button fetchPxButton = FindViewById<Button>(Resource.Id.FetchPatientsButton);
            fetchPxButton.Enabled = false;
            EditText patientNameField = FindViewById<EditText>(Resource.Id.PatientNameField);
            patientNameField.Enabled = false;
        }
    }
}

