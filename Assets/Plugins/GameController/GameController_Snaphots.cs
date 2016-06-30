//////////////////////////////////////////////////////////////////////////
/// @file	GameController_Snapshots.cs
///
/// @author	Sean Turner (ST)
///
/// @brief	Controller snapshot support for the Unity plugin interface to the native iOS7+ GameController.framework.
/// 
/// @note	Read the Apple GameController.framework documentation for details of the underlying functionality being exposed here.
/// 
/// @note	Snapshot usage:
/// 
///			 bool 	GetGamepadSnapshot(int nController, ref GCGamepadSnapShotDataV100 snapshot)
/// 		 bool 	GetExtendedGamepadSnapshot(int nController, ref GCExtendedGamepadSnapShotDataV100 snapshot)
///
///			 snapshot.GetInputValue(GCInput eInput) : return the value of a single input in this snapshot using the GCInput enum.
/// 
//////////////////////////////////////////////////////////////////////////

// ST: note currently using direct memory snapshots is very slow, because of the GCHandle.Alloc, so disabling for now
#define DIRECTMEMORYSNAPSHOTSx														// Capture snapshot directly into our classes from the GC framework

using UnityEngine;																	// Unity 			(ref http://docs.unity3d.com/Documentation/ScriptReference/index.html)
using System;																		// String / Math 	(ref http://msdn.microsoft.com/en-us/library/system.aspx)
using System.Collections;															// Queue 			(ref http://msdn.microsoft.com/en-us/library/system.collections.aspx)
using System.Collections.Generic;													// List<> 			(ref http://msdn.microsoft.com/en-us/library/system.collections.generic.aspx)
using System.Runtime.InteropServices;												// DllImport 		(ref http://msdn.microsoft.com/en-us/library/system.runtime.interopservices.aspx)

//////////////////////////////////////////////////////////////////////////
/// @brief GameController manager class.
//////////////////////////////////////////////////////////////////////////
public partial class GameController : MonoBehaviour
{
	const float 					k_fPressedThreshold = 0.1f;						//!< How far down a button must be to count as pressed

	/************************** SNAPSHOT CLASSES ****************************/

	// Gamepad snapshot class. Mirror of the struct of the same name declared inside GameController.framework
	[StructLayout(LayoutKind.Sequential, Pack=1)]
	public class GCGamepadSnapShotDataV100
	{
	    // Standard information
	    public Int16 version; //0x0100
	    public Int16 size;    //sizeof(GCGamepadSnapShotDataV100) or larger
	    
	    // Standard gamepad data
	    // Axes in the range [-1.0, 1.0]
	    public float dpadX;
	    public float dpadY;
	    
	    // Buttons in the range [0.0, 1.0]
	    public float buttonA;
	    public float buttonB;
	    public float buttonX;
	    public float buttonY;
	    public float leftShoulder;
	    public float rightShoulder;
		
		/// @brief	Get a snapshot input value by enum.
		public float GetInputValue(GCInput eInput)
		{
			switch(eInput)
			{
				case GCInput.gpDPadXAxis: 			return(dpadX);
				case GCInput.gpDPadYAxis: 			return(dpadY);
				case GCInput.gpDPadUp: 				return(Mathf.Max(0.0f, dpadY));
				case GCInput.gpDPadDown: 			return(Mathf.Max(0.0f, -dpadY));
				case GCInput.gpDPadLeft: 			return(Mathf.Max(0.0f, -dpadX));
				case GCInput.gpDPadRight: 			return(Mathf.Max(0.0f, dpadX));
				case GCInput.gpButtonA: 			return(buttonA);
				case GCInput.gpButtonB: 			return(buttonB);
				case GCInput.gpButtonX: 			return(buttonX);
				case GCInput.gpButtonY: 			return(buttonY);
				case GCInput.gpLeftShoulder: 		return(leftShoulder);
				case GCInput.gpRightShoulder: 		return(rightShoulder);
				default: 							return(0.0f);
			}
		}

		/// @brief	Set a snapshot input value by enum.
		public void SetInputValue(GCInput eInput, float fValue)
		{
			switch(eInput)
			{
				case GCInput.gpDPadXAxis: 			dpadX = fValue; break;
				case GCInput.gpDPadYAxis: 			dpadY = fValue; break;
				case GCInput.gpDPadUp: 				break;
				case GCInput.gpDPadDown: 			break;
				case GCInput.gpDPadLeft: 			break;
				case GCInput.gpDPadRight: 			break;
				case GCInput.gpButtonA: 			buttonA = fValue; break;
				case GCInput.gpButtonB: 			buttonB = fValue; break;
				case GCInput.gpButtonX: 			buttonX = fValue; break;
				case GCInput.gpButtonY: 			buttonY = fValue; break;
				case GCInput.gpLeftShoulder: 		leftShoulder = fValue; break;
				case GCInput.gpRightShoulder: 		rightShoulder = fValue; break;
				default: 							break;
			}
		}

		/// @brief	Get a snapshot input pressed status by enum.
		public bool GetInputPressed(GCInput eInput)
		{
			return( Mathf.Abs(GetInputValue(eInput)) > k_fPressedThreshold );
		}
	};
	
	// ExtendedGamepad snapshot class. Mirror of the struct of the same name declared inside GameController.framework
	[StructLayout(LayoutKind.Sequential, Pack=1)]
	public class GCExtendedGamepadSnapShotDataV100
	{
	    // Standard information
	    public Int16 version; //0x0100
	    public Int16 size;    //sizeof(GCExtendedGamepadSnapShotDataV100) or larger
	    
	    // Extended gamepad data
	    // Axes in the range [-1.0, 1.0]
	    public float dpadX;
	    public float dpadY;
	    
	    // Buttons in the range [0.0, 1.0]
	    public float buttonA;
	    public float buttonB;
	    public float buttonX;
	    public float buttonY;
	    public float leftShoulder;
	    public float rightShoulder;
	
	    // Axes in the range [-1.0, 1.0]
	    public float leftThumbstickX;
	    public float leftThumbstickY;
	    public float rightThumbstickX;
	    public float rightThumbstickY;
	    
	    // Buttons in the range [0.0, 1.0]
	    public float leftTrigger;
	    public float rightTrigger;
		
		/// @brief	Get a snapshot input value by enum.
		public float GetInputValue(GCInput eInput)
		{
			switch(eInput)
			{
				case GCInput.gpDPadXAxis: 			return(dpadX);
				case GCInput.gpDPadYAxis: 			return(dpadY);
				case GCInput.gpDPadUp: 				return(Mathf.Max(0.0f, dpadY));
				case GCInput.gpDPadDown: 			return(Mathf.Max(0.0f, -dpadY));
				case GCInput.gpDPadLeft: 			return(Mathf.Max(0.0f, -dpadX));
				case GCInput.gpDPadRight: 			return(Mathf.Max(0.0f, dpadX));
				case GCInput.gpButtonA: 			return(buttonA);
				case GCInput.gpButtonB: 			return(buttonB);
				case GCInput.gpButtonX: 			return(buttonX);
				case GCInput.gpButtonY: 			return(buttonY);
				case GCInput.gpLeftShoulder: 		return(leftShoulder);
				case GCInput.gpRightShoulder: 		return(rightShoulder);
				case GCInput.exLeftThumbstickXAxis: return(leftThumbstickX);
				case GCInput.exLeftThumbstickYAxis: return(leftThumbstickY);
				case GCInput.exLeftThumbstickUp: 	return(Mathf.Max(0.0f, leftThumbstickY));
				case GCInput.exLeftThumbstickDown: 	return(Mathf.Max(0.0f, -leftThumbstickY));
				case GCInput.exLeftThumbstickLeft: 	return(Mathf.Max(0.0f, -leftThumbstickX));
				case GCInput.exLeftThumbstickRight: return(Mathf.Max(0.0f, leftThumbstickX));
				case GCInput.exRightThumbstickXAxis:return(rightThumbstickX);
				case GCInput.exRightThumbstickYAxis:return(rightThumbstickY);
				case GCInput.exRightThumbstickUp: 	return(Mathf.Max(0.0f, rightThumbstickY));
				case GCInput.exRightThumbstickDown: return(Mathf.Max(0.0f, -rightThumbstickY));
				case GCInput.exRightThumbstickLeft: return(Mathf.Max(0.0f, -rightThumbstickX));
				case GCInput.exRightThumbstickRight:return(Mathf.Max(0.0f, rightThumbstickX));
				case GCInput.exLeftTrigger: 		return(leftTrigger);
				case GCInput.exRightTrigger: 		return(rightTrigger);
				default: 							return(0.0f);
			}
		}

		/// @brief	Set a snapshot input value by enum.
		public void SetInputValue(GCInput eInput, float fValue)
		{
			switch(eInput)
			{
				case GCInput.gpDPadXAxis: 			dpadX = fValue; break;
				case GCInput.gpDPadYAxis: 			dpadY = fValue; break;
				case GCInput.gpDPadUp: 				break;
				case GCInput.gpDPadDown: 			break;
				case GCInput.gpDPadLeft: 			break;
				case GCInput.gpDPadRight: 			break;
				case GCInput.gpButtonA: 			buttonA = fValue; break;
				case GCInput.gpButtonB: 			buttonB = fValue; break;
				case GCInput.gpButtonX: 			buttonX = fValue; break;
				case GCInput.gpButtonY: 			buttonY = fValue; break;
				case GCInput.gpLeftShoulder: 		leftShoulder = fValue; break;
				case GCInput.gpRightShoulder: 		rightShoulder = fValue; break;
				case GCInput.exLeftThumbstickXAxis: leftThumbstickX = fValue; break;
				case GCInput.exLeftThumbstickYAxis: leftThumbstickY = fValue; break;
				case GCInput.exLeftThumbstickUp: 	break;
				case GCInput.exLeftThumbstickDown: 	break;
				case GCInput.exLeftThumbstickLeft: 	break;
				case GCInput.exLeftThumbstickRight: break;
				case GCInput.exRightThumbstickXAxis:rightThumbstickX = fValue; break;
				case GCInput.exRightThumbstickYAxis:rightThumbstickY = fValue; break;
				case GCInput.exRightThumbstickUp: 	break;
				case GCInput.exRightThumbstickDown: break;
				case GCInput.exRightThumbstickLeft: break;
				case GCInput.exRightThumbstickRight:break;
				case GCInput.exLeftTrigger: 		leftTrigger = fValue; break;
				case GCInput.exRightTrigger: 		rightTrigger = fValue; break;
				default: 							break;
			}
		}

		/// @brief	Get a snapshot input pressed status by enum.
		public bool GetInputPressed(GCInput eInput)
		{
			return( Mathf.Abs(GetInputValue(eInput)) > k_fPressedThreshold );
		}
	};
	
	/************************** TAKING SNAPSHOTS  ***************************/
	
	//////////////////////////////////////////////////////////////////////////
	/// @brief	Take a snapshot of all Gamepad input values into a provided GCGamepadSnapShotDataV100 variable.
	//////////////////////////////////////////////////////////////////////////
	public static bool GetGamepadSnapshot(int nController, ref GCGamepadSnapShotDataV100 snapshot)
	{
		bool bSuccess = false;

#if UNITY_IPHONE
		if( Application.platform == RuntimePlatform.IPhonePlayer )
		{
#if DIRECTMEMORYSNAPSHOTS
			// Create a pinned GCHandle to stop the Garbage Collector moving the snapshot data during this call.
			GCHandle handle = GCHandle.Alloc(snapshot, GCHandleType.Pinned);
			bSuccess = _GCGetGamepadSnapshotV100(nController, handle.AddrOfPinnedObject());
			handle.Free();
#else // DIRECTMEMORYSNAPSHOTS
			// Set each input's value in the snapshot one-by-one
			for(int i=0; i<(int)GCInput.Last; i++)
				snapshot.SetInputValue( (GCInput)i, GetInputValue( nController, (GCInput)i ) );
			bSuccess = true;
#endif // DIRECTMEMORYSNAPSHOTS
		}
#endif // UNITY_IPHONE

		return(bSuccess);
	}
	
	//////////////////////////////////////////////////////////////////////////
	/// @brief	Take a snapshot of all ExtendedGamepad input values into a provided GCExtendedGamepadSnapShotDataV100 variable.
	//////////////////////////////////////////////////////////////////////////
	public static bool GetExtendedGamepadSnapshot(int nController, ref GCExtendedGamepadSnapShotDataV100 snapshot)
	{
		bool bSuccess = false;

#if UNITY_IPHONE
		if( Application.platform == RuntimePlatform.IPhonePlayer )
		{
#if DIRECTMEMORYSNAPSHOTS
			// Create a pinned GCHandle to stop the Garbage Collector moving the snapshot data during this call.
			GCHandle handle = GCHandle.Alloc(snapshot, GCHandleType.Pinned);
			bSuccess = _GCGetExtendedGamepadSnapshotV100(nController, handle.AddrOfPinnedObject());
			handle.Free();
#else // DIRECTMEMORYSNAPSHOTS	
			// Set each input's value in the snapshot one-by-one
			for(int i=0; i<(int)GCInput.Last; i++)
				snapshot.SetInputValue( (GCInput)i, GetInputValue( nController, (GCInput)i ) );
			bSuccess = true;
#endif // DIRECTMEMORYSNAPSHOTS
		}
#endif // UNITY_IPHONE

		return(bSuccess);
	}
	
	/************************* NATIVE IOS METHODS ***************************/

#if UNITY_IPHONE
	// iOS native interface
	[DllImport("__Internal")]
	private static extern bool _GCGetGamepadSnapshotV100(int nController, System.IntPtr outSnapshot);
	[DllImport("__Internal")]
	private static extern bool _GCGetExtendedGamepadSnapshotV100(int nController, System.IntPtr outSnapshot);
#endif // UNITY_IPHONE
}
