﻿using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;


public class CloudConnector : MonoBehaviour
{
	// -- Complete the following fields. --
	private string webServiceUrl = "https://script.google.com/macros/s/AKfycbwgQBKlG_Jg514ch8V6fYVhTXGiX1cZFdxyEvwZuUupfE7aFP8/exec";
	private string spreadsheetId = "1C0zyuPyQ0P-DxUbV4N7yiSA5P9XyBCV2V71c1noxfvU"; // If this is a fixed value could also be setup on the webservice to save POST request size.
	private string servicePassword = "passcode";
	private float timeOutLimit = 30f;
	public bool usePOST = true;
	// --
	
	private static CloudConnector _Instance;
	public static CloudConnector Instance
	{
		get
		{
            _Instance = new GameObject("CloudConnector").AddComponent<CloudConnector>();
            DontDestroyOnLoad(_Instance);
			return _Instance;
		}
	}
	
	private UnityWebRequest www;
	
	public void CreateRequest(Dictionary<string, string> form)
	{
		form.Add("ssid", spreadsheetId);
		form.Add("pass", servicePassword);
		
		if (usePOST)
		{
			CloudConnectorCore.UpdateStatus("Establishing Connection at URL " + webServiceUrl);
			www = UnityWebRequest.Post(webServiceUrl, form);
		}
		else // Use GET.
		{
			string urlParams = "?";
			foreach (KeyValuePair<string, string> item in form)
			{
				urlParams += item.Key + "=" + item.Value + "&";
			}
			CloudConnectorCore.UpdateStatus("Establishing Connection at URL " + webServiceUrl + urlParams);
			www = UnityWebRequest.Get(webServiceUrl + urlParams);
		}
		
		StartCoroutine(ExecuteRequest(form));
	}
	
	IEnumerator ExecuteRequest(Dictionary<string, string> postData)
	{
		www.Send();
		
		float elapsedTime = 0.0f;
		
		while (!www.isDone)
		{
			elapsedTime += Time.deltaTime;			
			if (elapsedTime >= timeOutLimit)
			{
				CloudConnectorCore.ProcessResponse("TIME_OUT", elapsedTime);
				break;
			}
			
			yield return null;
		}
		
		if (www.isError)
		{
			CloudConnectorCore.ProcessResponse(CloudConnectorCore.MSG_CONN_ERR + "Connection error after " + elapsedTime.ToString() + " seconds: " + www.error, elapsedTime);
			yield break;
		}	
		
		CloudConnectorCore.ProcessResponse(www.downloadHandler.text, elapsedTime);
	}
	
}

	