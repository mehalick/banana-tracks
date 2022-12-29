namespace BananaTracks {
	export class App {
		static playAudio(id: string): void {
			(document.getElementById(id) as HTMLAudioElement).play();
		}
		static getTimezoneOffset(): number {
			return new Date().getTimezoneOffset();
		}
	}
}
