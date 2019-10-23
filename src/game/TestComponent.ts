/**
 * @File   : TestComponent.ts
 * @Author : 瞬光 (shunguang.dty@alibaba-inc.com)
 * @Date   : 2019/10/23 下午2:58:06
 * 
 */
import * as Sein from 'seinjs';

export interface ITestComponentState {
  rotateSpeed: number;
}

export function isTestComponent(value: Sein.SObject): value is TestComponent {
  return (value as TestComponent).isTestComponent;
}

@Sein.SClass({className: 'TestComponent'})
export default class TestComponent extends Sein.Component<ITestComponentState> {
  public isTestComponent = true;

  private rotateSpeed: number = 0;

  public onAdd(initState: ITestComponentState) {
    this.rotateSpeed = initState.rotateSpeed;
  }

  public onUpdate(delta: number) {
    this.getOwner<Sein.SceneActor>().transform.rotationY += delta / 1000 * this.rotateSpeed;
  }
}
