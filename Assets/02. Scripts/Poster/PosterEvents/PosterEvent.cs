using MikroFramework.Architecture;

namespace _02._Scripts.Poster.PosterEvents {
	public interface IPosterEvent<T>  where T: Poster {
		T Poster{ get; set; }
		
		PosterModel PosterModel{ get;}
	}
	
	
	public static class PosterEventExtension {
		public static void AddPosterToModel<T>(this IPosterEvent<T> posterEvent, T poster) where T: Poster {
			posterEvent.PosterModel.AddPoster(poster);
		}
	}
}