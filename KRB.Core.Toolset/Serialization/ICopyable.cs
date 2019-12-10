namespace KRB.Core.Toolset.Serialization
{
   public interface ICopyable<in T>
   {
      void CopyTo(T target);
   }

   public interface IDeepCopyable<T> : ICopyable<T>
   {
      T DeepCopy();
   }

   public interface IShallowCopyable<T> : ICopyable<T>
   {
      T ShallowCopy();
   }
}
