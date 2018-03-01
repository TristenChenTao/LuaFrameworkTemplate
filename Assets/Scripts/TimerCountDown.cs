using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Timers;
using System.Linq;
using System.ComponentModel;
using LuaInterface;

public class TimerCountDown

{
    Timer timer;
    private int totalNum;
    private float delayTime;
    private LuaFunction callback;
    private LuaFunction endCallback;
    private int currentIndex = 0;

   public static TimerCountDown inst() {
       return new TimerCountDown();
   }

    public void CountDown(int totalNum, float delayTime, LuaFunction callback = null, LuaFunction endCallback = null)
    {
        Loom.Initialize();
        if(timer != null) {
            timer.Stop();
            timer  = null;
        }
        timer = new Timer(delayTime * 1000);
        timer.AutoReset = true;
        timer.Elapsed += TimerUp;
        timer.Enabled = true;

        this.currentIndex = totalNum;

        this.totalNum = totalNum;

        this.delayTime = delayTime;

        this.callback = callback;

        this.endCallback = endCallback;
    }

    /// <summary>
    /// Timer类执行定时到点事件       
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TimerUp(object sender, System.Timers.ElapsedEventArgs e)
    {
        try
        {

            if (currentIndex == 0 && endCallback != null)
            {  
                if (endCallback != null)
                {
                    Loom.QueueOnMainThread(() =>
                    {
                        endCallback.Call();
                    });
                }

                 Stop();
            }
            if (currentIndex >= 0 && callback != null)
            {
                Loom.QueueOnMainThread(() =>
                {
                    callback.Call(Math.Max(currentIndex,0));
                });

            }

            currentIndex -= 1;
        }
        catch (Exception ex)
        {
            Debug.Log("执行定时到点事件失败:" + ex.Message);
        }
    }

    public void Run()
    {
        this.timer.Start();
    }

    public void addSecond(int moreTime)
    {
        totalNum += moreTime;
        this.currentIndex += moreTime;
    }
    public void Stop()
    {
        timer.Stop();
    }
}