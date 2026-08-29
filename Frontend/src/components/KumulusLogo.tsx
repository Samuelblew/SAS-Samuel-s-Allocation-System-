import { Center, useGLTF } from '@react-three/drei'
import { Canvas, useFrame, useThree } from '@react-three/fiber'
import { Suspense, useCallback, useEffect, useRef, useState } from 'react'
import type { Group } from 'three'
import { MathUtils } from 'three'

const MODEL_PATH = '/brand/Kumulus.glb'
const FALLBACK_PATH = '/brand/kumulus.png'
const BASE_SCALE = 0.32
const HOVER_SCALE = BASE_SCALE * 1.06
const HOVER_TILT_RAD = MathUtils.degToRad(15)
const SPIN_SPEED = 4.2
const LERP_SPEED = 0.14

function KumulusModel({
  hovered,
  spinning,
  resetting,
  onLoaded,
  onResetComplete,
}: {
  hovered: boolean
  spinning: boolean
  resetting: boolean
  onLoaded: () => void
  onResetComplete: () => void
}) {
  const group = useRef<Group>(null)
  const { scene } = useGLTF(MODEL_PATH)

  useEffect(() => {
    onLoaded()
  }, [onLoaded, scene])

  useFrame((_, delta) => {
    const node = group.current
    if (!node) return

    if (spinning) {
      node.rotation.y += delta * SPIN_SPEED
      node.rotation.x = MathUtils.lerp(node.rotation.x, 0, LERP_SPEED)
      node.scale.setScalar(HOVER_SCALE)
      return
    }

    if (hovered) {
      node.rotation.y = MathUtils.lerp(node.rotation.y, HOVER_TILT_RAD, LERP_SPEED)
      node.rotation.x = MathUtils.lerp(node.rotation.x, 0, LERP_SPEED)
      node.scale.setScalar(MathUtils.lerp(node.scale.x, HOVER_SCALE, LERP_SPEED))
      return
    }

    node.rotation.y = MathUtils.lerp(node.rotation.y, 0, LERP_SPEED)
    node.rotation.x = MathUtils.lerp(node.rotation.x, 0, LERP_SPEED)
    node.scale.setScalar(MathUtils.lerp(node.scale.x, BASE_SCALE, LERP_SPEED))

    if (
      resetting &&
      Math.abs(node.rotation.y) < 0.002 &&
      Math.abs(node.rotation.x) < 0.002 &&
      Math.abs(node.scale.x - BASE_SCALE) < 0.002
    ) {
      node.rotation.y = 0
      node.rotation.x = 0
      node.scale.setScalar(BASE_SCALE)
      onResetComplete()
    }
  })

  return (
    <Center>
      <group ref={group} scale={BASE_SCALE}>
        <primitive object={scene} />
      </group>
    </Center>
  )
}

useGLTF.preload(MODEL_PATH)

function CanvasController({ active }: { active: boolean }) {
  const invalidate = useThree((state) => state.invalidate)

  useEffect(() => {
    if (!active) return
    invalidate()
  }, [active, invalidate])

  useFrame(() => {
    if (active) invalidate()
  })

  return null
}

export function KumulusLogo({ className = '' }: { className?: string }) {
  const [hovered, setHovered] = useState(false)
  const [spinning, setSpinning] = useState(false)
  const [resetting, setResetting] = useState(false)
  const [ready, setReady] = useState(false)

  const animating = hovered || spinning || resetting || !ready

  const handleClick = useCallback((event: React.MouseEvent) => {
    event.preventDefault()
    event.stopPropagation()
    setSpinning((current) => !current)
    setResetting(false)
  }, [])

  const handleMouseLeave = useCallback(() => {
    setHovered(false)
    setSpinning(false)
    setResetting(true)
  }, [])

  const handleResetComplete = useCallback(() => {
    setResetting(false)
  }, [])

  return (
    <div
      className={`ias-kumulus-logo ${className}`.trim()}
      onMouseEnter={() => setHovered(true)}
      onMouseLeave={handleMouseLeave}
      onClick={handleClick}
      role="presentation"
    >
      <img
        src={FALLBACK_PATH}
        alt=""
        className={`ias-kumulus-logo__fallback${ready ? ' ias-kumulus-logo__fallback--hidden' : ''}`}
      />
      <Canvas
        className="ias-kumulus-logo__canvas"
        camera={{ position: [0, 0, 2.6], fov: 38 }}
        dpr={[1, 2]}
        gl={{ alpha: true, antialias: true, powerPreference: 'high-performance' }}
        frameloop="demand"
        onCreated={({ gl }) => {
          gl.setClearColor(0x000000, 0)
        }}
      >
        <CanvasController active={animating} />
        <ambientLight intensity={0.7} />
        <directionalLight position={[4, 5, 6]} intensity={1.35} />
        <directionalLight position={[-3, -2, -4]} intensity={0.3} />
        <Suspense fallback={null}>
          <KumulusModel
            hovered={hovered}
            spinning={spinning}
            resetting={resetting}
            onLoaded={() => setReady(true)}
            onResetComplete={handleResetComplete}
          />
        </Suspense>
      </Canvas>
    </div>
  )
}
