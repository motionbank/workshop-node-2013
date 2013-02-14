#region usings
using System;
using System.ComponentModel.Composition;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


using VVVV.Core.Logging;
using System.Collections.Generic;

#endregion usings

namespace VVVV.Nodes
{
	
	[PluginInfo(Name = "JsonParser", Category = "JSON", Help = "parse json string", Tags = "")]
	public class JsonParser : IPluginEvaluate
	{
		#region fields & pins
		[Input("JSON", DefaultString = "hello")]
		IDiffSpread<string> FInput;
		
		[Input("Go", IsBang=true, IsSingle=true)]
		IDiffSpread<bool> FGo;
		
		[Output("Output json")]
		ISpread<JObject> FJOutput;
		
		[Output("status")]
		ISpread<string> FStatus;
		
		[Import()]
		ILogger FLogger;
		#endregion fields & pins
		//called when data for any output pin is requested
		
		public void Evaluate(int SpreadMax)
		{
			FJOutput.SliceCount = SpreadMax;
			FStatus.SliceCount = SpreadMax;
			
			if(FInput.SliceCount != 0 && FGo[0]==true)
			{
				for(int i=0; i< SpreadMax; i++)
				{
					try
					{
						FJOutput[i] = JObject.Parse(FInput[i]);
						FStatus[i] = "ok";
					}
					catch(Exception e)
					{
						FStatus[i] = "error";
					}
					
				}
			}
		}
	}
	
	[PluginInfo(Name = "SelectToken", Category = "JSON", Help = "select json token", Tags = "")]
	public class SelectToken : IPluginEvaluate
	{
		#region fields & pins
		[Input("JObject")]
		ISpread<JObject> FInput;
		
		[Input("path")]
		ISpread<string> FInputp;
		
		[Output("Output")]
		ISpread<string> FOutput;
		
		[Import()]
		ILogger FLogger;
		#endregion fields & pins
		//called when data for any output pin is requested
		
		public void Evaluate(int SpreadMax)
		{
			FOutput.SliceCount = SpreadMax;
			
			if( FInput[0]!=null){
				
				for (int i = 0; i < FInputp.SliceCount; i++)
				{
					if( FInput[0].SelectToken(FInputp[i])!= null && FInput[0]!=null)
					{
						FOutput[i]=  FInput[0].SelectToken(FInputp[i]).ToString();
					}
					else
					{
						FOutput[i]= "";
					}
				}
			}
		}
	}
	
	[PluginInfo(Name = "JsonArray", Category = "JSON", Help = "list json array content", Tags = "")]
	public class JsonArray : IPluginEvaluate
	{
		#region fields & pins
		[Input("Jobject")]
		IDiffSpread<JObject> FInput;
		
		[Input("path")]
		ISpread<string> FInputp;
		
		[Input("key")]
		ISpread<string> FInputk;
		
		[Output("Output")]
		ISpread<string> FOutput;
		
		[Output("count")]
		ISpread<int> FcOutput;
		
		[Import()]
		ILogger FLogger;
		#endregion fields & pins
		//called when data for any output pin is requested
		
		public void Evaluate(int SpreadMax)
		{
			
			FcOutput.SliceCount = SpreadMax;
			
			if(FInput.SliceCount != 0)
			{
				int i = 0;
				int divider = 1;
				
				for(int j=0; j< SpreadMax; j++)
				{
					
					if( FInput[j]!=null)
					{
						if( FInput[j].SelectToken(FInputp[0])!= null  )
						{
							var results  = FInput[j].SelectToken(FInputp[0]);
							foreach (JToken child in results.Children())
							{
								if( child.SelectToken(FInputk[0])!= null )
								{
									FOutput[i] = child.SelectToken(FInputk[0]).ToString();
								}
								else
								{
									FOutput[i] = "";
								}
								i++;
							}
							FOutput.SliceCount = i;
							FcOutput[j] = i / divider;
						}
					}
					else
					{
						FOutput.SliceCount = 1;
						FcOutput[0] = 0;
						FOutput[i]= "";
					}
					divider += 1;
					
				}
			}
		}
	}
}
