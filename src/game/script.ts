/**
 * @File   : script.tsx
 * @Author : 瞬光 (shunguang.dty@alibaba-inc.com)
 * @Date   : Wed Oct 23 2019
 * @Description: script.
 */
import * as Sein from 'seinjs';
import 'seinjs-camera-controls/lib/CameraOrbitControlComponent';

import './SeinTestExtension';
import GameState from './GameState';

/**
 * 游戏启动时触发。
 */
export async function onAdd(game: Sein.Game<GameState>) {

}

/**
 * 用于用户登录，异步。
 */
export async function onLogin(game: Sein.Game<GameState>) {

}

/**
 * 游戏资源预加载放在这里。
 */
export function onPreLoad(game: Sein.Game<GameState>) {
  game.resource.load({type: 'GlTF', name: 'main.gltf', url: require('assets/gltfs/main.gltf')});
}

/**
 * 更新游戏加载进度。
 */
export function onLoading(game: Sein.Game<GameState>, state: Sein.IResourceState) {
  console.log(state.current, state.progress);
}

/**
 * 资源加载完成后，创建相机、灯光，并实例化模型。
 */
export function onCreate(game: Sein.Game<GameState>) {
  createCamera(game);
  createLights(game);

  game.resource.instantiate<'GlTF'>('main.gltf');
}

/**
 * 每一帧更新。
 */
export function onUpdate(game: Sein.Game<GameState>, delta: number) {

}

/**
 * 游戏出错时触发。
 */
export function onError(error: Sein.BaseException, details: any) {
  
}

/**
 * 游戏销毁时触发。
 */
export function onDestroy(game: Sein.Game<GameState>) {

}

function createCamera(game: Sein.Game) {
  const {world} = game;

  const camera = world.addActor('camera', Sein.PerspectiveCameraActor, {
    far: 1000,
    near: .01,
    fov: 60,
    aspect: game.screenWidth / game.screenHeight,
    position: new Sein.Vector3(0, 0, -10)
  });
  camera.lookAt(new Sein.Vector3(0, 0, 0));

  camera.addComponent('control', Sein.CameraControls.CameraOrbitControlComponent, {
    enableDamping: true,
    dampingFactor: .2,
    zoomMax: 100,
    zoomMin: .1,
    target: new Sein.Vector3(0, 0, 0)
  });
}

function createLights(game: Sein.Game) {
  const {world} = game;

  world.addActor('aLight', Sein.AmbientLightActor, {
    color: new Sein.Color(1, 1, 1),
    amount: .5
  });
  world.addActor('dLight', Sein.DirectionalLightActor, {
    direction: new Sein.Vector3(0, -1, 1),
    color: new Sein.Color(1, 1, 1),
    amount: 2
  });
}
