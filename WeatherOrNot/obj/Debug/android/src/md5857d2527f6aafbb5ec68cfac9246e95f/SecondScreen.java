package md5857d2527f6aafbb5ec68cfac9246e95f;


public class SecondScreen
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("WeatherOrNot.SecondScreen, WeatherOrNot, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", SecondScreen.class, __md_methods);
	}


	public SecondScreen () throws java.lang.Throwable
	{
		super ();
		if (getClass () == SecondScreen.class)
			mono.android.TypeManager.Activate ("WeatherOrNot.SecondScreen, WeatherOrNot, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
