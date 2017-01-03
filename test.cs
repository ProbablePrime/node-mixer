/*
  LICENSE
  -------
  Copyright (C) 2007-2010 Ray Molenkamp

  This source code is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this source code or the software it produces.

  Permission is granted to anyone to use this source code for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:

  1. The origin of this source code must not be misrepresented; you must not
     claim that you wrote the original source code.  If you use this source code
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.
  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original source code.
  3. This notice may not be removed or altered from any source distribution.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreAudio;
using System.Diagnostics;
using System.Threading.Tasks;

using System.Collections.Generic;
public class Startup {
    public async Task<object> setMutePID(dynamic input) {
        int pid = (int)input.pid;
        bool mute = (bool)input.mute;
        return Mute.setMutePID(pid,mute);
    }
    public async Task<object> setVolumeForPID(dynamic input) {
        int pid = (int)input.pid;
        float vol = (float)input.volume;
        return Mute.setVolumeForPID(pid,vol);
    }
    public async Task<object> getDevices(dynamic input) {
        return Mute.getDevices();
    }
}

static class Mute {
  public static List<Details> list = new List<Details>{};
  public static MMDevice getDevice() {
    MMDeviceEnumerator DevEnum = new MMDeviceEnumerator();
    MMDevice device = DevEnum.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
    return device;
  }
  public static AudioSessionControl2 getSessionForProcessID(int pid) {
    MMDevice device = Mute.getDevice();
    for (int i = 0; i < device.AudioSessionManager2.Sessions.Count; i++) {
      AudioSessionControl2 session = device.AudioSessionManager2.Sessions[i];
      if(session.GetProcessID == pid) {
        return session;
      }
    }
    return null;
  }
  public static  SimpleAudioVolume getVolumeForProcessID(int pid) {
    AudioSessionControl2 session = Mute.getSessionForProcessID(pid);
    if(session != null) {
      return session.SimpleAudioVolume;
      //AudioMeterInformation mi = session.AudioMeterInformation;
    }
    return null;
  }
  public static bool setVolumeForPID(int pid, float volume) {
    SimpleAudioVolume res = Mute.getVolumeForProcessID(pid);
    if(res != null) {
      res.MasterVolume = volume;
    }
    return true;
  }
  public static bool setMutePID(int pid, bool mute) {
    SimpleAudioVolume res = Mute.getVolumeForProcessID(pid);
    if(res != null) {
      res.Mute = mute;
    }
    return mute;
  }
  public static Details[] getDevices() {
    Mute.list.Clear();
    MMDevice device = Mute.getDevice();
    for (int i = 0; i < device.AudioSessionManager2.Sessions.Count; i++) {
      try {
        Mute.list.Add(new Details(device.AudioSessionManager2.Sessions[i]));
      } catch(ArgumentException e) {

      }
    }
    return Mute.list.ToArray();
  }
}

public class Details {
  public float volume;
  public String title;
  public String name;
  public int id;
  public bool muted;

  public Details(AudioSessionControl2 session) {
    Process p = Process.GetProcessById((int)session.GetProcessID);
    id = (int)session.GetProcessID;
    title = p.MainWindowTitle;
    name = p.ProcessName;
    SimpleAudioVolume simpleVolume = session.SimpleAudioVolume;
    muted = simpleVolume.Mute;
    volume = simpleVolume.MasterVolume;
  }
}
