using UnityEngine;

public class ProgressionController
{
    private ProgressionView _view;
    private ProgressionModel _model;
    
    public void Initialize(ProgressionView view, ProgressionModel model)
    {
        _view = view;
        _model = model;
        _view.Initialize(model.DisplayValue, model.MaXValue);
    }
    
    //TODO add UniTask to advance the value 
}
