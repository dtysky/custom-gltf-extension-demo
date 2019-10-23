/**
 * @File   : SeinTestExtension.ts
 * @Author : dtysky (dtysky@outlook.com)
 * @Date   : 2019/10/23 下午2:58:39
 * @Description:
 */
import * as Sein from 'seinjs';

import TestComponent from './TestComponent';

export interface ISeinTestExtensionInfo {
  rotateSpeed: number;
}

const SeinTestExtension: Sein.IGlTFExtension<ISeinTestExtensionInfo> = {
  name: 'Sein_test',
  instantiate(entity: Sein.SceneActor | Sein.SceneComponent, info: ISeinTestExtensionInfo, game: Sein.Game) {
    if (!Sein.isSceneActor(entity)) {
      Sein.Debug.warn(`You could not add physicBody to a component: ${entity.name}, ignore...`);
      return;
    }

    entity.addComponent('test', TestComponent, info);
  }
}

Sein.GlTFLoader.REGISTER_EXTENSION(SeinTestExtension);

export default SeinTestExtension;
