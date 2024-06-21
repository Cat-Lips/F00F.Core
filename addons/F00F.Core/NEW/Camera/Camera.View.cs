namespace F00F;

public partial class Camera
{
    private void InitView()
    {
        ViewChanged += InitShape;
        GetViewport().SizeChanged += ViewChanged;
    }

    private float _fov;
    private float _near;
    private void UpdateView()
    {
        if (_fov != Camera3D.Fov || _near != Camera3D.Near)
        {
            _fov = Camera3D.Fov;
            _near = Camera3D.Near;
            ViewChanged();
        }
    }

}
