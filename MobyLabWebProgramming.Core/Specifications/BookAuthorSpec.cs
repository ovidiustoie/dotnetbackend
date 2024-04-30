using Ardalis.Specification;
using MobyLabWebProgramming.Core.Entities;

public class BookAuthorSpec<T> : Specification<T>  where T: BookAuthor{
    public sealed override ISpecificationBuilder<T> Query => base.Query;
    public BookAuthorSpec(Guid BookId) => Query.Where(e => e.BookId == BookId);
  }
