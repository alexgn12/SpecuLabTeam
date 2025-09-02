import { platformBrowser } from '@angular/platform-browser';
import { AppModule } from './app/app-module';
import { registerables } from 'chart.js';
import { Chart } from 'chart.js';
import { provideAnimations } from '@angular/platform-browser/animations';


Chart.register(...registerables);

platformBrowser().bootstrapModule(AppModule, {
  ngZoneEventCoalescing: true,
  providers: [provideAnimations()]
})
  .catch(err => console.error(err));
  
