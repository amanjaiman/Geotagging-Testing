using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class LocationScript : MonoBehaviour
{

    private TextMesh UIText;
    private double latitude;
    private double longitude;

    private double irbLat = 38.989364;
    private double irbLong = -76.936197;
    //private double radius = 0.00063043239129243;
    private double radius = 0.0573112657515379;

    //private double rLat = 38.989618;
    //private double rLong = -76.936774;

    private int c = 1;

    double GetDistance(double lat1, double lon1, double lat2, double lon2) 
    {
        var R = 6371; // Radius of the earth in km
        var dLat = ToRadians(lat2-lat1);
        var dLon = ToRadians(lon2-lon1); 
        var a = 
                Math.Sin(dLat/2) * Math.Sin(dLat/2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) * 
                Math.Sin(dLon/2) * Math.Sin(dLon/2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1-a)); 
        var d = R * c; // Distance in km
        return d;
    }

    double ToRadians(double deg) 
    {
        return deg * (Math.PI/180);
    }

    private void Start()
    {
        UIText = (TextMesh)GameObject.Find("UIText").GetComponent<TextMesh>();

        if (!Permission.HasUserAuthorizedPermission (Permission.FineLocation))
        {
                Permission.RequestUserPermission (Permission.FineLocation);
        }
        StartCoroutine(StartLocationService());
    }

    private IEnumerator StartLocationService()
    {
        if (!Input.location.isEnabledByUser)
        {
                UIText.text = "Location is not enabled";
                yield break;
        }
        Input.location.Start();
        while(Input.location.status == LocationServiceStatus.Initializing)
        {
                yield return new WaitForSeconds(1);
        }
        if (Input.location.status == LocationServiceStatus.Failed)
        {
                UIText.text = "Unable to determine device location";
                yield break;
        }
        else
        {
            InvokeRepeating("checkLocation", 1, 3);
            //checkLocation();
        }
    }

    private void checkLocation() {
        latitude = Input.location.lastData.latitude;
        longitude = Input.location.lastData.longitude;
        UIText.text = "Location: " + latitude + ", " + longitude;

        var distance = GetDistance(latitude, longitude, irbLat, irbLong);

        if (distance < radius)
        {
            inIRB();
        }
        else
        {
            UIText.text = c + "\n" + "You are not in IRB" + "\n" + "Distance: " + distance;
            c = c + 1;
        }
    }

    private void inIRB() {
        // use google calendar api to get events happening in IRB
        UIText.text = "You are in IRB"; //temp
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
