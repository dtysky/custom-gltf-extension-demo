/**
 * @File   : main.tsx
 * @Author : dtysky (dtysky@outlook.com)
 * @Date   : Wed Oct 23 2019
 * @Description: Component.
 */
import * as Sein from 'seinjs';

import GameState from './GameState';
import * as script from './script';

export async function main(canvas: HTMLCanvasElement): Promise<Sein.Game> {
  const engine = new Sein.Engine();

  const game = new Sein.Game(
    'intro-game',
    {
      canvas,
      clearColor: new Sein.Color(0, .6, .9, 1),
      width: canvas.offsetWidth,
      height: canvas.offsetHeight,
      pixelRatio: window.devicePixelRatio
    },
    GameState
  );

  engine.addGame(game);

  game.addWorld('main', Sein.GameModeActor, MainLevelScript);
  game.onError = script.onError;

  await game.start();

  return game;
}

class MainLevelScript extends Sein.LevelScriptActor {
  public onAdd() {
    script.onAdd(this.getGame());
  }

  public async onLogin() {
    await script.onLogin(this.getGame());
  }

  public onPreload() {
    script.onPreLoad(this.getGame());
  }

  public onLoading(state: Sein.IResourceState) {
    script.onLoading(this.getGame(), state);
  }

  public onCreate() {
    script.onCreate(this.getGame());
  }

  public onUpdate(delta: number) {
    script.onUpdate(this.getGame(), delta);
  }

  public onDestroy() {
    script.onDestroy(this.getGame());
  }
}
