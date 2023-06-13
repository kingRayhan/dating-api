using api.Data;

namespace api.Utils;

public class HasDataContext
{
    protected readonly DataContext _dataContext;

    public HasDataContext(DataContext dataContext)
    {
        _dataContext = dataContext;
    }
}