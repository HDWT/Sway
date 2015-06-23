# Sway

How to use:

1) Simple move <br>
Sway.MoveTo(transform, Vector3.one, 1);

2) Start look at point after 4 seconds <br>
Sway.LookAt(transform, Vector3.one, 2).Delay(4);

3) Scale up using custom easeType and then scale down <br>
Sway.ScaleTo(transform, Vector3.one * 2, 2).EaseType(m_curve).OnComplete((overtime) => <br>
{ <br>
  Sway.ScaleTo(transform, Vector3.one, 2).Delay(-overtime); <br>
}); <br>
