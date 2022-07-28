using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uMVVM.Sources.Infrastructure;

public class HPAndMPViewModel : ViewModelBase
{
    public readonly BindableProperty<int> HP = new BindableProperty<int>();
    public readonly BindableProperty<int> MP = new BindableProperty<int>();
    public readonly BindableProperty<string> Name = new BindableProperty<string>();
    



}
